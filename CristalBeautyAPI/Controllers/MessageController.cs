using Microsoft.AspNetCore.Mvc;
using CristalBeautyAPI.Models.Dto;
using CristalBeautyAPI.Helpers;
using CristalBeautyAPI.Models;
using CristalBeautyAPI.Services.IServices;
using CristalBeautyAPI.Utility;

namespace CristalBeautyAPI.Controllers;

[ApiController]
[Route("api/message")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDTO dto)
    {
        try
        {
            string messageId = await _messageService.CreateMessageAsync(dto);
            return StatusCode(201, new { message = "Message sent successfully", messageId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to send message" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages()
    {
        try
        {
            Dictionary<string, object?>? user = UserHelper.GetUser(HttpContext);
            if (user == null || UserHelper.GetRole(user) != SD.Role_Admin)
                return StatusCode(403, new { message = "Unauthorized - Admin access required" });

            List<Message> messages = await _messageService.GetAllMessagesAsync();
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to fetch messages" });
        }
    }

    [HttpPut("{messageId}")]
    public async Task<IActionResult> UpdateMessageStatus(string messageId, [FromBody] UpdateMessageStatusDTO dto)
    {
        try
        {
            Dictionary<string, object?>? user = UserHelper.GetUser(HttpContext);
            if (user == null || UserHelper.GetRole(user) != SD.Role_Admin)
                return StatusCode(403, new { message = "Unauthorized - Admin access required" });

            Message? message = await _messageService.UpdateMessageStatusAsync(messageId, dto.Status);
            if (message == null) return NotFound(new { message = "Message not found" });

            return Ok(new { message = "Status updated successfully", data = message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to update message" });
        }
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage(string messageId)
    {
        try
        {
            Dictionary<string, object?>? user = UserHelper.GetUser(HttpContext);
            if (user == null || UserHelper.GetRole(user) != SD.Role_Admin)
                return StatusCode(403, new { message = "Unauthorized - Admin access required" });

            Message? message = await _messageService.DeleteMessageAsync(messageId);
            if (message == null) return NotFound(new { message = "Message not found" });

            return Ok(new { message = "Message deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to delete message" });
        }
    }

    [HttpPost("{messageId}/reply")]
    public async Task<IActionResult> ReplyToMessage(string messageId, [FromBody] ReplyToMessageDTO dto)
    {
        try
        {
            Dictionary<string, object?>? user = UserHelper.GetUser(HttpContext);
            if (user == null || UserHelper.GetRole(user) != SD.Role_Admin)
                return StatusCode(403, new { message = "Unauthorized - Admin access required" });

            await _messageService.ReplyToMessageAsync(messageId, dto.Subject, dto.ReplyMessage);
            return Ok(new { message = "Reply sent successfully" });
        }
        catch (ArgumentException ex)
        {
             return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to send reply: " + (ex.Message ?? "Unknown error") });
        }
    }
}
