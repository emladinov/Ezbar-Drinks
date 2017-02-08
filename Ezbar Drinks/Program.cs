using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
/*
 * using System.Text;
 * using System.Threading.Tasks;
 * using System.Data.SqlClient;
*/

namespace ezbar_drinks
{
    public class ConnectionFactory
    {
        /* The ConnectionFactory class is used throughout the program to connect to the database. Instead of having to write out the credentials
         * and create a connection everytime, the line 'var connection = ConnectionFactory.Create();' is used in each function. This line makes
         * the variable 'connection' the connection string to the database.
         */
        public static MySqlConnection Create()
        {
            MySqlConnection connection;
            string server = "ezbar.nyc",
                database = "",
                user = "",
                password = "";

            string connectionString = @"server=" + server + ";" + "UID=" + user + ";" + "password=" + password + ";" + "database=" + database + ";";
            connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

            }
            // Exception Handling
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        break;

                    case 1045:
                        break;
                }
            }
            return connection;
        }
    }
    /* The class Program has all of the functions that get the necessary information from the database in order to 
     * examine which recipes are available.
     */
    class Program
    {
        //This function gets the inventory (which is inputted from the user).
        public static string[] getInventory()
        {
            int counter = 0;
            int tally = 0;
            Console.Out.Write("Please enter ingredients in your inventory. ");
            Console.Out.WriteLine("Seperate each by a comma:\n");
            string unparsedInventory;
            unparsedInventory = Console.ReadLine(); //variable is set as input from user.
            char[] delimeterChars = { ',', '.', ':', '\t' }; //characters that are parsed out of the input.
            string[] inventory = unparsedInventory.Split(delimeterChars);

            //This for loop puts each string seperated by a delimeter character into an element of the array.
            foreach (string s in inventory)
            {
                inventory[counter] = s;
                counter += 1;
            }
            tally = counter;
            counter = 0;
            Console.Out.WriteLine("\nThese are the values in the inventory array:");

            //This loop outputs the inventory in a list in the console.
            while (tally > counter)
            {
                Console.Out.WriteLine(inventory[counter]);
                counter += 1;
            }
            Console.WriteLine();
            return inventory;
        }


        public static void compare()
        {
            var connection = ConnectionFactory.Create();
            string[] inventory = getInventory();
            int numofrecipes = getnumofrecipes();
            int noi = 0;
            string[] recipe = new string[20];
            bool availability = true;
            int inventorycounter = 0;

            for (int recipenumber = 1; recipenumber <= numofrecipes; recipenumber++)
            {
                string noiquery = "SELECT NOI From Recipes WHERE R_PK = " + recipenumber + ";";
                MySqlCommand noicmd = new MySqlCommand(noiquery, connection);
                List<String> list = new List<String>();
                MySqlDataReader noiDataReader = noicmd.ExecuteReader();

                while (noiDataReader.Read())
                {
                    list.Add(noiDataReader["NOI"] + "");
                }
                noiDataReader.Close();
                noi = Int32.Parse(list.First());

                if (noi >= 1)
                {
                    string firstingredientquery = "SELECT Ingredient from 1st_Ingredient where R_PK =" + recipenumber + ";";
                    MySqlCommand firstIngcmd = new MySqlCommand(firstingredientquery, connection);
                    MySqlDataReader firstIngDataReader = firstIngcmd.ExecuteReader();
                    firstIngDataReader.Read();
                    recipe[0] = (firstIngDataReader["Ingredient"] + "");
                    firstIngDataReader.Close();
                }

                if (noi >= 2)
                {
                    string secondingredientquery = "SELECT Ingredient from 2nd_Ingredient where R_PK =" + recipenumber + ";";
                    MySqlCommand secondIngcmd = new MySqlCommand(secondingredientquery, connection);
                    MySqlDataReader secondIngDataReader = secondIngcmd.ExecuteReader();
                    secondIngDataReader.Read();
                    recipe[1] = (secondIngDataReader["Ingredient"] + "");
                    secondIngDataReader.Close();
                }

                if (noi >= 3)
                {
                    string thirdingredientquery = "SELECT Ingredient from 3rd_Ingredient where R_PK =" + recipenumber + ";";
                    MySqlCommand thirdIngcmd = new MySqlCommand(thirdingredientquery, connection);
                    MySqlDataReader thirdIngDataReader = thirdIngcmd.ExecuteReader();
                    thirdIngDataReader.Read();
                    recipe[2] = (thirdIngDataReader["Ingredient"] + "");
                    thirdIngDataReader.Close();
                }


                if (noi >= 4)
                {
                    string fourthingredientquery = "SELECT Ingredient from 4th_Ingredient where R_PK =" + recipenumber + ";";
                    MySqlCommand fourthIngcmd = new MySqlCommand(fourthingredientquery, connection);
                    MySqlDataReader fourthIngDataReader = fourthIngcmd.ExecuteReader();
                    fourthIngDataReader.Read();
                    recipe[3] = (fourthIngDataReader["Ingredient"] + "");
                    fourthIngDataReader.Close();
                }

                if (noi >= 5)
                {
                    string fifthingredientquery = "SELECT Ingredient from 5th_Ingredient where R_PK =" + recipenumber + ";";
                    MySqlCommand fifthIngcmd = new MySqlCommand(fifthingredientquery, connection);
                    MySqlDataReader fifthIngDataReader = fifthIngcmd.ExecuteReader();
                    fifthIngDataReader.Read();
                    recipe[4] = (fifthIngDataReader["Ingredient"] + "");
                    fifthIngDataReader.Close();
                }

                //checks whther teh elements on the recipe array match the elements in the inventory array

                for (int i = 0; i < noi; i++)
                {
                    availability = false;
                    for (inventorycounter = 0; inventorycounter < inventory.Length; inventorycounter++)
                    {
                        if (recipe[i] == inventory[inventorycounter])
                        {
                            availability = true;
                            break;
                        }

                        if (availability == false && inventorycounter == (inventory.Length - 1))
                            goto Foo;
                    }
                }

                Foo:

                if (availability == true)
                    Console.WriteLine(getnameofrecipe(recipenumber) + " is available");
                else
                    Console.WriteLine(getnameofrecipe(recipenumber) + " is not available");
            }
        }

        public static int getnumofrecipes()
        {
            int counter = 0;
            var connection = ConnectionFactory.Create();
            string counterquery = "SELECT COUNT(*) FROM Recipes;";
            MySqlCommand countercmd = new MySqlCommand(counterquery, connection);
            counter = Convert.ToInt32(countercmd.ExecuteScalar());

            return counter;
        }

        public static string getnameofrecipe(int numofrecipe)
        {
            var connection = ConnectionFactory.Create();
            string recipename = "";
            string recipenamequery = "SELECT Recipe_Name FROM Recipes WHERE R_PK =" + numofrecipe + ";";
            MySqlCommand namecmd = new MySqlCommand(recipenamequery, connection);
            recipename = (namecmd.ExecuteScalar().ToString());

            return recipename;
        }

        static void Main(string[] args)
        {
            compare();
        }
    }
}
