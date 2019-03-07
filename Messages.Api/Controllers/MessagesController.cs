using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages.Api.Helpers;
using Messages.Api.Models;
using Messages.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messages.Api.Controllers
{
    [Route("messages"), ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messagesService;

        public MessagesController(IMessageService service)
        {
            _messagesService = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadMessageViewModel>>> Get()
        {
            var messages = await _messagesService.GetAll();
            return messages.Select(CreateReadModel).ToList();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ReadMessageViewModel>> Get(Guid id)
        {
            var message = await _messagesService.GetById(id);
            if (message == null)
            {
                return NotFound();
            }

            return CreateReadModel(message);
        }

        [HttpPost]
        public async Task<IActionResult> Post(WriteMessageViewModel model)
        {
            var newMessage = new Message
            {
                Value = model.Message,
                IsPalindrome = model.Message.IsPalindrome()
            };

            newMessage = await _messagesService.Add(newMessage);

            return CreatedAtAction(nameof(Get), new { id = newMessage.Id }, CreateReadModel(newMessage));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, WriteMessageViewModel model)
        {
            var existingMessage = await _messagesService.GetById(id);
            if (existingMessage == null)
            {
                return NotFound();
            }

            existingMessage.Value = model.Message;
            existingMessage.IsPalindrome = model.Message.IsPalindrome();

            await _messagesService.Update(existingMessage);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingMessage = await _messagesService.GetById(id);
            if (existingMessage == null)
            {
                return NotFound();
            }

            await _messagesService.Delete(existingMessage);

            return NoContent();
        }

        /// <summary>
        /// Maps the Message entity (which is saved to the database) to a model returned to clients.
        /// </summary>
        private ReadMessageViewModel CreateReadModel(Message message) => new ReadMessageViewModel
        {
            Id = message.Id,
            Message = message.Value,
            IsPalindrome = message.IsPalindrome
        };
    }
}