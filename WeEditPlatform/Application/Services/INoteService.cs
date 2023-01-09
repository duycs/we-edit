using Application.Models;
using Domain;

namespace Application.Services
{
    public interface INoteService
    {
        Note AddNote(CreateNoteVM request);
        Note UpdateNote(UpdateNoteVM request);
        void RemoveNote(int id);

        Note FindNote(int id);
        List<Note> FindNotesByObject(string objectName, int objectId);
        List<Note> FindNotes(int[] ids);
        List<NoteDto> FindNotes(int pageNumber, int pageSize, string objectName, int[]? objectIds, string searchValue, out int totalRecords);
    }
}
