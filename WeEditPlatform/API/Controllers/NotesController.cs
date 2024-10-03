using Application.Models;
using Application.Services;
using Domain;
using Infrastructure.Pagging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Policy = "apiPolicy")]
    [ApiController]
    [Route("[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly ILogger<NotesController> _logger;
        private readonly IUriService _uriService;
        private readonly INoteService _noteService;

        public NotesController(ILogger<NotesController> logger,
            IUriService uriService, INoteService noteService)
        {
            _logger = logger;
            _uriService = uriService;
            _noteService = noteService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetNotes([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 0,
           [FromQuery] string? objectName = "", [FromQuery] int[]? objectIds = null, [FromQuery] string? searchValue = "", [FromQuery] int[]? ids = null)
        {
            if (ids != null && ids.Any())
            {
                var notes = _noteService.FindNotes(ids);
                return Ok(notes);
            }
            else
            {
                var paggedJobNoteDtos = _noteService.FindNotes(pageNumber, pageSize, objectName, objectIds, searchValue, out int totalRecords);
                var jobPagedReponse = PaginationHelper.CreatePagedReponse<NoteDto>(paggedJobNoteDtos, new PaginationFilterOrder(pageNumber, pageSize),
                    totalRecords, _uriService, Request.Path.Value);


                return Ok(jobPagedReponse);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetNote(int id)
        {
            var note = _noteService.FindNote(id);
            return Ok(note);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddNote([FromBody] CreateNoteVM request)
        {
            var noteCreated = _noteService.AddNote(request);
            return Ok(noteCreated);
        }

        [Authorize]
        [HttpPut]
        public IActionResult UpdateNote([FromBody] UpdateNoteVM request)
        {
            var noteUpdated = _noteService.UpdateNote(request);
            return Ok(noteUpdated);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult RemoveNote(int id)
        {
            _noteService.RemoveNote(id);
            return Ok();
        }
    }
}
