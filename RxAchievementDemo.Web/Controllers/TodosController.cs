using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using RxAchievementDemo.Web.Models;
using EasyNetQ;
using EasyNetQ.Topology;
using RxAchievementDemo.Models;

namespace RxAchievementDemo.Web.Controllers
{
    public class TodosController : ApiController
    {
        static readonly IAdvancedBus s_bus = RabbitHutch.CreateBus().Advanced;
        static readonly IExchange s_exchange = Exchange.DeclareTopic("RCC_RX");

        private TodoContext db = new TodoContext();

        // GET api/Todos
        public IEnumerable<Todo> GetTodoes()
        {
            return db.Todoes.AsEnumerable();
        }

        // GET api/Todos/5
        public Todo GetTodo(int id)
        {
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return todo;
        }

        // PUT api/Todos/5
        public HttpResponseMessage PutTodo(int id, Todo todo)
        {
            if (ModelState.IsValid && id == todo.Id)
            {
                db.Entry(todo).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();

                    RaiseTodoEvent(id, EventType.Update);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // POST api/Todos
        public HttpResponseMessage PostTodo(Todo todo)
        {
            if (ModelState.IsValid)
            {
                db.Todoes.Add(todo);
                db.SaveChanges();

                RaiseTodoEvent(todo.Id, EventType.Add);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, todo);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = todo.Id }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Todos/5
        public HttpResponseMessage DeleteTodo(int id)
        {
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Todoes.Remove(todo);
            RaiseTodoEvent(id, EventType.Delete);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todo);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public void RaiseTodoEvent(int id, EventType type)
        {
            var todoEvent = new TodoEvent()
            {
                Id = id,
                Type = type
            };

            var message = new Message<TodoEvent>(todoEvent);
            
            var routing = "Task." + type.ToString();

            using(var channel = s_bus.OpenPublishChannel())
            {
                channel.Publish(s_exchange, routing, message);
            }
        }
    }
}