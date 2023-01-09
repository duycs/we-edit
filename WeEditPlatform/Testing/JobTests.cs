using Domain;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    public class JobTests : TestBase
    {

        [Test]
        public void Add_Steps_To_Job()
        {
            var job = new Job();
            job.AddStep(new Step() { Id = 1 });
            job.AddStep(new Step() { Id = 2 });

            Assert.NotNull(job);
            Assert.AreEqual(2, job.Steps.Count);
            Assert.True(job.Steps.Any(w => w.Id == 1));
            Assert.True(job.Steps.Any(w => w.Id == 2));
        }
    }
}
