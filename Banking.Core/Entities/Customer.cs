using Banking.Core.Entities;

public class Customer
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private Customer(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Customer Create(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");

        return new Customer(id, name);
    }
}