namespace MockDoor.Client.Models
{
    public class EnumItem<T> where T : struct, Enum
    {
        public T? EnumValue { get; set; }

        public string EnumName { get; set; }
    }
}
