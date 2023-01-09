using Application.Models;
using Application.Queries;
using Domain;
using Infrastructure.Repository;

namespace Application.Services
{
    public class NoteService : INoteService
    {
        private readonly IRepositoryService _repositoryService;

        public NoteService(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Note AddNote(CreateNoteVM request)
        {
            var note = Note.Create(request.NoterId, request.Title, request.Description, request.ObjectName, request.ObjectId);
            var noteCreated = _repositoryService.Add(note);
            _repositoryService.SaveChanges();

            return noteCreated;
        }

        public Note FindNote(int id)
        {
            var noteExisting = _repositoryService.Find<Note>(id);

            if (noteExisting == null) throw new Exception($"Note {id} does not existing");

            return noteExisting;
        }

        public List<Note> FindNotes(int[] ids)
        {
            var notes = _repositoryService.List<Note>(ids);

            if (notes == null || !notes.Any()) throw new Exception($"Notes does not existing");

            return notes;
        }

        public List<NoteDto> FindNotes(int pageNumber, int pageSize, string objectName, int[]? objectIds, string searchValue, out int totalRecords)
        {
            NoteSpecification noteSpecification;
            if (!string.IsNullOrEmpty(objectName) && objectIds != null && objectIds.Any())
            {
                noteSpecification = new NoteSpecification(objectName, objectIds, searchValue);

            }
            else
            {
                noteSpecification = new NoteSpecification(objectName, objectIds);
            }

            var paggedNotes = _repositoryService.Find<Note>(pageNumber, pageSize, noteSpecification, out totalRecords).ToList();

            // mapping noter
            var staffIds = paggedNotes.Select(n => n.NoterId).ToList();
            var noters = _repositoryService.List<Staff>(s => staffIds.Contains(s.Id));

            var paggedNoteDtos = new List<NoteDto>();
            paggedNotes.ForEach(note =>
            {
                var noter = noters.FirstOrDefault(n => n.Id == note.NoterId);
                var noteDto = new NoteDto();
                noteDto.Note = note;
                noteDto.Noter = noter;

                paggedNoteDtos.Add(noteDto);
            });

            return paggedNoteDtos;
        }

        public List<Note> FindNotesByObject(string objectName, int objectId)
        {
            return _repositoryService.List<Note>(n => n.ObjectName.ToLower() == objectName.ToLower() && n.ObjectId == objectId).ToList();
        }

        public void RemoveNote(int id)
        {
            var noteExisting = _repositoryService.Find<Note>(id);

            if (noteExisting == null) throw new Exception($"Note {id} does not existing");

            _repositoryService.Delete(noteExisting);
            _repositoryService.SaveChanges();
        }

        public Note UpdateNote(UpdateNoteVM request)
        {
            var noteExisting = _repositoryService.Find<Note>(request.NoteId);

            if (noteExisting == null) throw new Exception($"Note {request.NoteId} does not existing");

            noteExisting.UpdateNote(request.NoterId, request.Title, request.Description, request.ObjectName, request.ObjectId);

            var noteUpdated = _repositoryService.Update(noteExisting);
            _repositoryService.SaveChanges();

            return noteUpdated;
        }
    }
}
