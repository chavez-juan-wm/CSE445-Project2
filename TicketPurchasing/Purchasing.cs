using System;
using System.Threading;

namespace eCommerce 
{ 
    // Define delegates for events
    public delegate void priceCutEvent(String parkName, Int32 newPrice);
    public delegate void purchaseTicketEvent(int numTickets);

    public class MultiCellBuffer
    {
        //Need to implement data cells don't really know what these are specifically, are they just OrderObjects? 
        //From specs we need to implement a semaphore and need a lock mechanism for this, this will most likely take the most time out of the project. <<<
        public void setOneCell()
        {

        }
        public void getOneCell() //Don't know what return type it should be yet.
        {

        }
    }

    public class OrderProcessing
    {
        private string senderId;
        private int cardNo;
        private string receiverID;
        private int amount;
        private double unitPrice;

        public string getSenderId() { return senderId; } //Have to decide if the getters and setters need to be synchronous, assuming they should as we don't want the price to fluctuate when making
        public int getCardNo() { return cardNo; }        //a purchase.<<<
        public string getReceiverID() { return receiverID; }
        public int getAmount() { return amount; }
        public double getUnitPrice() { return unitPrice; }

        public void setSenderId(string newSender) { senderId = newSender; }
        public void setCardNo(int newCardNo) { cardNo = newCardNo; }
        public void setReceiverId(string newReceiver) { receiverID = newReceiver; }
        public void setAmount(int newAmount) { amount = newAmount; }
        public void setUnitPrice(double newUnitPrice) { unitPrice = newUnitPrice; }
        public void orderFunc()
        {
            //Send the order to Multi-cell buffer?<<<
        }

    }

    public class Park1
    {
        static Random rng = new Random();
        public static event priceCutEvent PriceCut; // Link event to delegate
        private static Int32 ticketPrice = 10;
        private static int numCuts = 0;
        private static int currentDay = 0;

        public Int32 GetPrice() 
        { 
            return ticketPrice; 
        } 

        public static void ChangePrice(Int32 price) 
        {
            // The new price is a price cut 
            if (price < ticketPrice)
            {
                // There is at least 1 subscriber 
                if (PriceCut != null)
                {
                    // emit event to subscribers
                    PriceCut(Thread.CurrentThread.Name, price);
                    numCuts++;
                }
            }
            ticketPrice = price;
        } 

        public void SupplierFunc() 
        { 
            while(numCuts < 25) 
            { 
                Thread.Sleep(500); 
                // Take the order from the queue of the orders; 
                // Decide the price based on the orders 
                
                Int32 r = rng.Next(1, 20);
                // Console.WriteLine("New Price is {0}", p); 
                Park1.PriceModel(currentDay,r);
                currentDay++;
                currentDay %= 5;
            } 
        } 
        public static void PriceModel(int weekday, int numberOfTickets) //Need to have two price models for different parks<<<
        {
            int newprice = 80;
            int[] dayFee = { 5, 6, 7, 8, 9 };

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
            Park1.ChangePrice(newprice);
        }
    }

    public class Park2
    {
        static Random rng = new Random();
        public static event priceCutEvent PriceCut; // Link event to delegate
        private static Int32 ticketPrice = 10;
        private static int numCuts = 0;
        private static int currentDay = 0;
        public Int32 GetPrice()
        {
            return ticketPrice;
        }
        public static void ChangePrice(Int32 price)
        {
            // The new price is a price cut 
            if (price < ticketPrice)
            {
                // There is at least 1 subscriber 
                if (PriceCut != null)
                {
                    // emit event to subscribers
                    PriceCut(Thread.CurrentThread.Name, price);  
                    numCuts++;
                }
            }
            ticketPrice = price;
        }
        public void SupplierFunc()
        {
            while (numCuts < 25)
            {
                Thread.Sleep(500);
                // Take the order from the queue of the orders; 
                // Decide the price based on the orders 
                
                Int32 r = rng.Next(1, 20);
                // Console.WriteLine("New Price is {0}", p); 
                Park2.PriceModel(currentDay, r);
                currentDay++;
                currentDay %= 5;
            }
        }
        public static void PriceModel(int weekday, int numberOfTickets) //Need to have two price models for different parks<<<
        {
            int newprice = 80;
            int[] dayFee = { 5, 6, 7, 8, 9 };

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
            Park2.ChangePrice(newprice);
        }
    }

    public class TicketAgency //Need to add a purchase event
    {
        private int numOfOrders = 0;
        private int stockOfTickets = 0;
       
        public static event purchaseTicketEvent purchaseTicket; //Link ticket purchase event<<<
        public void NewOrder()
        {
           numOfOrders++;
        }
        public void TicketAgencyFunc() 
        { 
            // Each ticket agency has no ticket stock for each park
            // Order tickets from each park to sell
            // Create a loop that makes orders for each agency
            

        } 
        public void OrderTickets(int numOfTickets) 
        {
            OrderProcessing newOrder = new OrderProcessing();
            Thread order = new Thread(new ThreadStart(newOrder.orderFunc));
            order.Name = ("Order#" + (numOfOrders + 1)).ToString(); //Name should hopefully be Order#1 
            order.Start();
        }

        // Event handler for price cut
        public void TicketOnSale(String parkName, Int32 newPrice) 
        { 
            // order tickets from Park – send order into queue 
            // Need to decide if we need to purchase more tickets depending on inventory
            // And check if it is worth purchasing more
            // If they have less than 150 tickets then purchase more tickets to get 300 again

            // profit = priceofticket + .5*priceofticket
            Console.WriteLine("{0} tickets are on sale as low as ${1} each", parkName, newPrice);  // It prints thread name 
        } 
    }
    
    public class eCommerceApp
    {
        // Main func instantiates the objects and threads
        static void Main(string[] args)
        {
            // Instantiate two different park objects
            Park1 park1 = new Park1();
            Park2 park2 = new Park2();

            // Create two threads for each park
            Thread park1Thread = new Thread(new ThreadStart(park1.SupplierFunc));
            park1Thread.Name = "Park 1";
            park1Thread.Start();

            Thread park2Thread = new Thread(new ThreadStart(park2.SupplierFunc));
            park2Thread.Name = "Park 2";
            park2Thread.Start();

            // Create a new TicketAgency object so we can instantiate 5 Ticket Agency threads
            TicketAgency ticketstore = new TicketAgency();

            // Subscribe to event
            Park1.PriceCut += new priceCutEvent(ticketstore.TicketOnSale);
            Park2.PriceCut += new priceCutEvent(ticketstore.TicketOnSale);

            // N agencies
            int N = 5;
            Thread[] ticketAgencyThreads = new Thread[5];
            for (int i = 0; i < N; i++)
            {
                ticketAgencyThreads[i] = new Thread(new ThreadStart(ticketstore.TicketAgencyFunc));
                ticketAgencyThreads[i].Name = "TicketAgency" + (i + 1).ToString();
                ticketAgencyThreads[i].Start();
            }
        }
    }

}