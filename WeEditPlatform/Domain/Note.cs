namespace Domain
{
    public class Note : EntityBase
    {
        public int NoterId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Note for objects are Job, JobStep, Staff,...
        /// </summary>
        public string ObjectName { get; set; }
        public int ObjectId { get; set; }

        public static Note Create(int noterId, string title, string description,
            string objectName, int objectId)
        {
            return new Note()
            {
                NoterId = noterId,
                Title = title,
                Description = description,
                ObjectName = objectName,
                ObjectId = objectId
            };
        }

        public Note UpdateNote(int noterId, string title, string description,
            string objectName, int objectId)
        {
            if (noterId > 0)
            {
                NoterId = noterId;
            }

            if (!string.IsNullOrEmpty(title))
            {
                Title = title;
            }

            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }

            if (!string.IsNullOrEmpty(objectName))
            {
                ObjectName = objectName;
            }

            if (objectId > 0)
            {
                ObjectId = objectId;
            }

            return this;
        }

        public Note SetNoteForObject(string objectName, int objectId)
        {
            ObjectName = objectName;
            ObjectId = objectId;
            return this;
        }
    }
}
