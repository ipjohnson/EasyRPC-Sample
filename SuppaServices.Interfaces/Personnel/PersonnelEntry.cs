using System;
using System.Collections.Generic;
using System.Text;

namespace SuppaServices.Interfaces.Personnel
{
    public class PersonnelEntry
    {
        public int PersonnelId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? EmploymentDate { get; set; }
    }
}
