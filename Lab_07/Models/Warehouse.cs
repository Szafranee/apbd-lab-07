namespace Lab_07.Models;

public class Warehouse
{
    public Warehouse() { }

    public Warehouse(int id, string name, string address)
    {
        Id = id;
        Name = name;
        Address = address;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}