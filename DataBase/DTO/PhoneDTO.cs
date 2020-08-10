using System;
using System.Collections.Generic;
using System.Text;

namespace DataBase.DTO
{
    public class PhoneDTO
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
