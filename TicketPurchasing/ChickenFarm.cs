using System;
using System.Threading;
namespace eCommerce 
{ 
    public delegate void priceCutEvent(Int32 pr); // Define a delegate 
    public delegate void purchaseTicketEvent(int numTickets);//Event for purchasing tickets, don't know what pr is for just copied from the other event

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

    public class Main
    {
        public void mainFunc() //The main func instantiates the objects we will be running
        {                      //Will need to rework this code by making Parks and OrderProcessing server side.<<<
            Park park = new Park(); //Create a Park object so that we can then instantiate 2 Park threads
            Thread[] parks = new Thread[2];
            for (int i = 0; i < 2; i++)
            {
                // N = 2 here
                // Start N retailer threads 
                parks[i] = new Thread(new ThreadStart(park.supplierFunc));
                parks[i].Name = (i + 1).ToString();
                parks[i].Start();
            }
            TicketAgency ticketstore = new TicketAgency(); //Create a new TicketAgency object so we can instantiate 5 Ticket Agency threads
            Park.priceCut += new priceCutEvent(ticketstore.ticketOnSale); //Do we need this? I don't think we do >>>
            Thread[] retailers = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                // N = 5 here
                // Start N retailer threads 
                retailers[i] = new Thread(new ThreadStart(ticketstore.ticketAgencyFunc));
                retailers[i].Name = (i + 1).ToString();
                retailers[i].Start();
            }
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
        public void supplierFunc() 
        { 
            while(numCuts < 25) 
            { 
                Thread.Sleep(500); 
                // Take the order from the queue of the orders; 
                // Decide the price based on the orders 
                Int32 p = rng.Next(0, 4); //Think we might need to have a different way to set up week days other than randomly generating the day. <<<
                Int32 r = rng.Next(1, 20);
                // Console.WriteLine("New Price is {0}", p); 
                Park.priceModel(p,r); 
            } 
        } 
        public static void priceModel(int weekday, int numberOfTickets) //Need to have two price models for different parks<<<
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
            Park.changePrice(newprice);
        }
    }
    

    public class TicketAgency //Need to add a purchase event
    {
        private int numOfOrders = 0;
        public static event purchaseTicketEvent purchaseTicket;//Link ticket purchase event<<<
        public void newOrder()
        {
           numOfOrders++;
        }
        public void ticketAgencyFunc() 
        { 
            //for starting thread 
            Park park = new Park(); //Need to find out if there is way we can get rid of this. Feel like it's kind of redundant to make a new park when we should be getting the price-
                                    //from one(maybe both) of the established park threads.<<<
            for (Int32 i = 0; i < 10; i++) 
            { 
                Thread.Sleep(1000);
                Int32 p = park.getPrice();
                Console.WriteLine("Store{0} has everyday low price: ${1} each", Thread.CurrentThread.Name, p);
                // Thread.CurrentThread.Name prints thread name 
            } 
        } 
        public void orderTickets(int numOfTickets) 
        {
            OrderProcessing newOrder = new OrderProcessing();
            Thread order = new Thread(new ThreadStart(newOrder.orderFunc));
            order.Name = ("Order#" + (numOfOrders + 1)).ToString();//Name should hopefully be Order#1 
            order.Start();
        }
        public void ticketOnSale(Int32 p) 
        { // Event handler 
        // order tickets from Park – send order into queue 
        Console.WriteLine("Thread {0}: tickets are on sale: as low as ${1} each", Thread.CurrentThread.Name, p);  // It prints thread name 
        } 
    }
    

    public class myApplication 
    { 
        static void Main(string[] args) 
        {
            //Park park = new Park();    //Commented this out until we can for sure delete it <<<
            //Thread[] parks = new Thread[2];
            //for (int i = 0; i < 2; i++)
            //{
            //    // N = 2 here
            //    // Start N retailer threads 
            //    parks[i] = new Thread(new ThreadStart(park.supplierFunc));
            //    parks[i].Name = (i + 1).ToString();
            //    parks[i].Start();
            //}
            //TicketAgency ticketstore = new TicketAgency();
            //Park.priceCut += new priceCutEvent(ticketstore.ticketOnSale);
            //Thread[] retailers = new Thread[5];
            //for (int i = 0; i < 5; i++) {
            //    // N = 5 here
            //    // Start N retailer threads 
            //    retailers[i] = new Thread(new ThreadStart(ticketstore.ticketAgencyFunc));
            //    retailers[i].Name = (i + 1).ToString();
            //    retailers[i].Start();
            //}
        } 
    }
    

}