﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Events
{
    public abstract class DomainEvent : INotification
    {
        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>The created.</value>
        [JsonIgnore]
        public DateTime Created { get; private set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [JsonIgnore]
        public string Type { get; set; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public Dictionary<string, object> Args { get; private set; }

        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>The correlation identifier.</value>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        protected DomainEvent()
        {
            Created = DateTime.Now;
            Args = new Dictionary<string, object>();
            Type = GetType().Name;
        }

        /// <summary>
        /// Flattens this instance.
        /// </summary>
        public abstract void Flatten();
    }
}
