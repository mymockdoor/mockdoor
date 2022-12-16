using System;
using System.ComponentModel.DataAnnotations;

namespace MockDoor.Shared.Models.Headers
{
    public class ServiceHeaderDto : ICopyTo<ServiceHeaderDto>
    {
        [MaxLength(150, ErrorMessage = "Header name exceeded max length {0}")]
        public string Name { get; set; }

        public bool Enabled { get; set; }

        public bool Incoming { get; set; }

        public bool Outgoing { get; set; }

        public ServiceHeaderDto()
        {
        }

        public ServiceHeaderDto(ServiceHeaderDto serviceHeader)
        {
            Name = serviceHeader.Name;
            Enabled = serviceHeader.Enabled;
            Incoming = serviceHeader.Incoming;
            Outgoing = serviceHeader.Outgoing;
        }

        public ServiceHeaderDto CopyTo(ServiceHeaderDto target)
        {
            if (target == null)
                throw new NotSupportedException($"{nameof(ServiceHeaderDto)}: Cannot copy to a null target");
            
            target.Name = Name;
            target.Enabled = Enabled;
            target.Incoming = Incoming;
            target.Outgoing = Outgoing;
            return target;
        }
    }
}