namespace TravelExpertsData;

/// <summary>
/// CustomerManager class provides methods for customer authentication, registration, updating, and finding.
/// </summary>
public class CustomerManager
{
    /// <summary>
    /// Authenticates a customer based on provided credentials.
    /// </summary>
    /// <param name="db">The database context to be used for querying customers.</param>
    /// <param name="username">Username as string</param>
    /// <param name="password">Password as string</param>
    /// <returns>A user object if exists, otherwise null.</returns>
    public static Customer Authenticate(TravelExpertsContext db, string username, string password)
    {
        // Find the customer using the provided username and password
        Customer? customer = db.Customers.SingleOrDefault(c => c.CustUsername == username && c.CustPassword == password);
        return customer!;
    }

    /// <summary>
    /// Registers a new customer to the database.
    /// </summary>
    /// <param name="db">The database context to be used for adding new customers.</param>
    /// <param name="customer">Customer object to be registered.</param>
    public static void Register(TravelExpertsContext db, Customer customer)
    {
        db.Customers.Add(customer);
        db.SaveChanges();
    }

    /// <summary>
    /// Updates an existing customer in the database.
    /// </summary>
    /// <param name="db">The database context to be used for updating customers.</param>
    /// <param name="customer">Customer object to be updated.</param>
    public static void Update(TravelExpertsContext db, Customer customer)
    {
        db.Customers.Update(customer);
        db.SaveChanges();
    }

    /// <summary>
    /// Finds a customer by their ID.
    /// </summary>
    /// <param name="db">The database context to be used for querying customers.</param>
    /// <param name="id">ID of the customer to be found.</param>
    /// <returns>A customer object if exists, otherwise null.</returns>
    public static Customer Find(TravelExpertsContext db, int id)
    {
        Customer? customer = db.Customers.Find(id);
        return customer!;
    }
}