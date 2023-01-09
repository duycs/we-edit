using Application.Models;
using Application.Services;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;

namespace Testing
{
    public class NoteServiceTest : TestBase
    {
        private INoteService _noteService;

        [OneTimeSetUp]
        public void Init()
        {
            _noteService = _serviceProvider.GetService<INoteService>();

            Init_First_Note();
        }

        [Test]
        [TestCase(1)]
        public void Find_Note_Success(int noteId)
        {
            var note = _noteService.FindNote(noteId);

            Assert.NotNull(note);
            Assert.AreEqual(note.Id, noteId);
        }

        [Test]
        [TestCase(1, 1000, "JobStep", "")]
        public void Find_Note_Of_JobStep_Success(int pageNumber, int pageSize, string objectName, int[] objectIds, string searchValue)
        {
            var notes = _noteService.FindNotes(pageNumber, pageSize, objectName, objectIds, searchValue, out int totalPages).ToList();

            Assert.NotNull(notes);
            Assert.True(notes.Any());
        }

        [Test]
        [TestCase(1, 1000, "JobStep", "")]
        public void Find_Note_Of_JobStep_Not_Found_Success(int pageNumber, int pageSize, string objectName, int[] objectIds, string searchValue)
        {
            var notes = _noteService.FindNotes(pageNumber, pageSize, objectName, objectIds, searchValue, out int totalPages).ToList();

            Assert.NotNull(notes);
            Assert.False(notes.Any());
        }

        [Test]
        [TestCase(1, "title2", "description2", "Staff", 1)]
        public void Add_Note_Success(int noterId, string title, string description, string objectName, int objectId)
        {
            var noteCreated = _noteService.AddNote(new CreateNoteVM()
            {
                NoterId = noterId,
                Title = title,
                Description = description,
                ObjectName = objectName,
                ObjectId = objectId
            });

            Assert.NotNull(noteCreated);
            Assert.AreEqual(title, noteCreated.Title);
        }

        [Test]
        [TestCase(1, 100, "title updated", "description updated", "Staff", 1)]
        public void Update_Note_Success(int noteId, int noterId, string title,
            string description, string objectName, int objectId)
        {
            var note = _repositoryService.Find<Note>(noteId);

            var updateNoteVM = new UpdateNoteVM()
            {
                NoteId = note.Id,
                NoterId = noterId,
                Title = title,
                Description = description,
                ObjectName = objectName,
                ObjectId = objectId
            };

            var noteUpdated = _noteService.UpdateNote(updateNoteVM);

            Assert.NotNull(noteUpdated);
            Assert.AreEqual(1, noteUpdated.Id);
            Assert.AreEqual(noterId, noteUpdated.NoterId);
            Assert.AreEqual(title, noteUpdated.Title);
            Assert.AreEqual(description, noteUpdated.Description);
            Assert.AreEqual(objectName, noteUpdated.ObjectName);
            Assert.AreEqual(objectId, noteUpdated.ObjectId);
        }

        private void Init_First_Note()
        {
            var note = Note.Create(1, "title1", "description1", "JobStep", 1);
            _repositoryService.Add(note);
            _repositoryService.SaveChanges();
        }
    }
}
