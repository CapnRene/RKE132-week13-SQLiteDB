using System.Data.SQLite;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

ReadData(CreateConnection());
//InsertCustomer(CreateConnection());
//RemoveCustomer(CreateConnection());
FindCustomer(CreateConnection());

static SQLiteConnection CreateConnection()
{
    SQLiteConnection connection = new SQLiteConnection("Data Source=mydb.db;Version=3;New=True;Compress=True");

    try
    {
        connection.Open();
        Console.WriteLine("Connection opened successfully.");
    }
    catch (SQLiteException ex)
    {
        Console.WriteLine($"SQLite error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"General error: {ex.Message}");
    }
    finally
    {
        //connection.Close();
        //Console.WriteLine("Connection closed.");
    }
    return connection;
}

static void ReadData(SQLiteConnection myConnection) { 
  Console.Clear();
    Console.WriteLine("Reading data from the database...");
    SQLiteDataReader reader;
    SQLiteCommand command;

    command = myConnection.CreateCommand();
    command.CommandText = "SELECT rowid, * FROM customer";
    reader = command.ExecuteReader();

    while (reader.Read())
    {
        int readerRowId = reader.GetInt32(0);
        string firstName = reader.GetString(1);
        string lastName = reader.GetString(2);
        string readStringDoB = reader.GetString(3);

        Console.WriteLine($"{readerRowId}. Customer: {firstName} {lastName}. DOB: {readStringDoB}");
    }
    reader.Close();

    command.CommandText = "SELECT customer.rowid, customer.firstName, customer.lastName, status.statustype FROM customerStatus " +
        "JOIN customer ON customer.rowid = customerStatus.customerId " +
        "JOIN status ON status.rowid = customerStatus.statusId";
    reader = command.ExecuteReader();

    while (reader.Read())
    {
        //string readerRowId = reader.["rowid"].ToString();
        int readerRowId = reader.GetInt32(0);
        string readStringFirstName = reader.GetString(1);
        string readStringLastName = reader.GetString(2);
        string readStringDoB = reader.GetString(3);

        Console.WriteLine($"{readerRowId}. Full name: {readStringFirstName} {readStringLastName}; Status: {readStringDoB}");
    }
    reader.Close();

    myConnection.Close();
    Console.WriteLine("Connection closed.");
}

static void InsertCustomer(SQLiteConnection myConnection)
{
    SQLiteCommand command;
    string fName, lName, dob;
    Console.WriteLine("Enter first name:");
    fName = Console.ReadLine();
    Console.WriteLine("Enter last name:");
    lName = Console.ReadLine();
    Console.WriteLine("Enter date of birth (mm-dd-yyyy):");
    dob = Console.ReadLine();

    command = myConnection.CreateCommand();
    command.CommandText = "INSERT INTO customer (firstName, lastName, dateOfBirth) " +
        $"VALUES (@{fName}, @{lName}, @{dob})";

    //command.Parameters.AddWithValue("@fName", fName);
    //command.Parameters.AddWithValue("@lName", lName);
    //command.Parameters.AddWithValue("@dob", dob);

    int rowInserted = command.ExecuteNonQuery();
    Console.WriteLine($"Row inserted: {rowInserted}");


    ReadData(myConnection);
}

static void RemoveCustomer(SQLiteConnection myConnection) {
    SQLiteCommand command;
    string idToDelete;
    Console.WriteLine("Enter the ID of the customer to delete:");
    idToDelete = Console.ReadLine();

    command = myConnection.CreateCommand();
    command.CommandText = $"DELETE FROM customer WHERE rowid = {idToDelete}";
    int rowDeleted = command.ExecuteNonQuery();
    Console.WriteLine($"Row deleted: {rowDeleted}");
}

static void FindCustomer(SQLiteConnection myConnection)
{
    SQLiteCommand command;
    string idToFind;
    Console.WriteLine("Enter the ID of the customer to find:");
    idToFind = Console.ReadLine();
    command = myConnection.CreateCommand();
    command.CommandText = $"SELECT rowid, * FROM customer WHERE rowid = {idToFind}";
    SQLiteDataReader reader = command.ExecuteReader();
    if (reader.Read())
    {
        int readerRowId = reader.GetInt32(0);
        string firstName = reader.GetString(1);
        string lastName = reader.GetString(2);
        string readStringDoB = reader.GetString(3);
        Console.WriteLine($"{readerRowId}. Customer: {firstName} {lastName}. DOB: {readStringDoB}");
    }
    else
    {
        Console.WriteLine("Customer not found.");
    }
}