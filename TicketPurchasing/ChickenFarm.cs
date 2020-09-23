using System;
using System.Threading;
namespace eCommerce 
{ 
    public delegate void priceCutEvent(Int32 pr); // Define a delegate 
    public class OrderProcessing
    {
        public void orderFunc()
        {

        }
    }
    public class Park
    { 
        static Random rng = new Random(); // To generate random numbers
        public static event priceCutEvent priceCut; // Link event to delegate
        private static Int32 ticketPrice = 10;
        private static int numCuts = 0;
        public Int32 getPrice() 
        { 
            return ticketPrice; 
        } 
        public static void changePrice(Int32 price) 
        {
            if (price < ticketPrice) { // a price cut 
                if (priceCut != null)
                {// there is at least a subscriber 
                    priceCut(price); // emit event to subscribers 
                    numCuts++;
                }
            } 
            ticketPrice = price;
        } 
        public void farmerFunc() 
        { 
            while(numCuts < 25) 
            { 
                Thread.Sleep(500); 
                // Take the order from the queue of the orders; 
                // Decide the price based on the orders 
                Int32 p = rng.Next(0, 4);
                Int32 r = rng.Next(1, 20);
                // Console.WriteLine("New Price is {0}", p); 
                Park.priceModel(p,r); 
            } 
        } 
        public static void priceModel(int weekday, int numberOfTickets)
        {
            int newprice = 80;
            int[] dayFee = { 5, 6, 7, 8, 9 };
            if (poop)
            {
                if (numberOfTickets >= 1 && numberOfTickets <= 4)
                {
                    newprice = numberOfTickets * dayFee[weekday] + 80;
                }
                else if (numberOfTickets >= 5 && numberOfTickets <= 15)
                {
                    newprice = numberOfTickets * dayFee[weekday] + 60;

                }
                else if (numberOfTickets >= 16 && numberOfTickets <= 20)
                {
                    newprice = numberOfTickets * dayFee[weekday] + 40;
                }
            }
            else
            {

            }
            Park.changePrice(newprice);
        }
    } 
    public class TicketAgency 
    { 
        public void ticketAgencyFunc() 
        { 
            //for starting thread 
            Park park = new Park();
            for (Int32 i = 0; i < 10; i++) 
            { 
                Thread.Sleep(1000);
                Int32 p = park.getPrice();
                Console.WriteLine("Store{0} has everyday low price: ${1} each", Thread.CurrentThread.Name, p);
                // Thread.CurrentThread.Name prints thread name 
            } 
        } 
        public void ticketOnSale(Int32 p) 
        { // Event handler 
        // order chickens from chicken farm – send order into queue 
        Console.WriteLine("Thread {0}: tickets are on sale: as low as ${1} each", Thread.CurrentThread.Name, p);  // It prints thread name 
        } 
    } 
    public class myApplication 
    { 
        static void Main(string[] args) 
        {
            Park park = new Park();
            Thread[] parks = new Thread[2];
            for (int i = 0; i < 2; i++)
            {
                // N = 2 here
                // Start N retailer threads 
                parks[i] = new Thread(new ThreadStart(park.farmerFunc));//change name of farmer func
                parks[i].Name = (i + 1).ToString();
                parks[i].Start();
            }
            TicketAgency ticketstore = new TicketAgency();
            Park.priceCut += new priceCutEvent(ticketstore.ticketOnSale);
            Thread[] retailers = new Thread[5];
            for (int i = 0; i < 5; i++) {
                // N = 5 here
                // Start N retailer threads 
                retailers[i] = new Thread(new ThreadStart(ticketstore.ticketAgencyFunc));
                retailers[i].Name = (i + 1).ToString();
                retailers[i].Start();
            }
        } 
    } 
}