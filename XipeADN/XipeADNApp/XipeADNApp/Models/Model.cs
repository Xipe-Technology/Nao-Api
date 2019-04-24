using System;
namespace XipeADNApp.Models
{
    public abstract class Model<T>
    {
        public T Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastEditDate { get; set; }
    }
}
