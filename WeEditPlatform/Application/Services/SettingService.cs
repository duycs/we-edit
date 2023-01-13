using Domain;
using Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SettingService : ISettingService
    {
        private readonly IRepositoryService _repsitoryService;

        public SettingService(IRepositoryService repsitoryService)
        {
            _repsitoryService = repsitoryService;
        }

        public Setting GetSettingByKey(string key)
        {
            return _repsitoryService.Find<Setting>(s => s.Key == key);
        }
    }
}
