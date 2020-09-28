using System;
using System.Threading;
using System.Globalization;
using System.Data;

namespace eCommerce
{ 
    // Define delegates for events
    public delegate void priceCutEvent(String parkName, double newPrice); //Emits to agencies of a price cut
    public delegate void orderEvent1(int index); //Emits to Park 1 that there is an order
    public delegate void orderEvent2(int index); //Emits to Park 2 that there is an order

    public static class MultiCellBuffer
    {
        private static Semaphore dataCells;
        private static Order[] orders;
        private static int numberOfOrders;
        public static event orderEvent1 OrderInQueue1;
        public static event orderEvent2 OrderInQueue2;

        public static void init(orderEvent1 park1, orderEvent2 park2)
        {
            numberOfOrders = 0;
            orders = new Order[3];
            dataCells = new Semaphore(0, 3);
            dataCells.Release(3);
            OrderInQueue1 += park1;
            OrderInQueue2 += park2;
        }

        // Write data into one of the available cells
        public static void setOneCell(Order order)
        {
            Console.WriteLine("{0}", Thread.CurrentThread.Name);
            dataCells.WaitOne();
            
            lock(orders)
            {
                orders[numberOfOrders] = order;
                numberOfOrders++;

                // Notify the park that there is an order that must be processed
                if (order.getReceiverID().Equals("Park 1"))
                    OrderInQueue1((numberOfOrders-1));
                if (order.getReceiverID().Equals("Park 2"))
                    OrderInQueue2((numberOfOrders-1));  
            }
        }

        // Read data from one of the available cells
        public static Order getOneCell(int index)
        {
            Order processOrder;
            lock (orders[index])
            {
                processOrder = orders[index];
            }
            dataCells.Release();
            numberOfOrders--;

            return processOrder;
        }
    }

    public class Order
    {
        private string senderId;
        private int cardNo;
        private string receiverID;
        private int amount;
        private double unitPrice;
        private DateTime startTime;

        public string getSenderId() { return senderId; }
        public int getCardNo() { return cardNo; }        //a purchase.<<<
        public string getReceiverID() { return receiverID; }
        public int getAmount() { return amount; }
        public double getUnitPrice() { return unitPrice; }
        public DateTime getStartTime() { return startTime; }

        public void setSenderId(string newSender) { senderId = newSender; }
        public void setCardNo(int newCardNo) { cardNo = newCardNo; }
        public void setReceiverId(string newReceiver) { receiverID = newReceiver; }
        public void setAmount(int newAmount) { amount = newAmount; }
        public void setUnitPrice(double newUnitPrice) { unitPrice = newUnitPrice; }
        public void setStartTime(DateTime newTime) { startTime = newTime; }
    }

    public class Park1
    {
        static Random rng = new Random();
        public static event priceCutEvent PriceCut;
        private static double ticketPrice = 80.0;
        private static int numCuts = 0;
        private static int currentDay = 0;
        private static int availableTickets = 500;
        

        public double GetPrice() 
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
                Park1.PriceModel(currentDay);
                currentDay++;
                currentDay %= 5;
            } 
        } 

        public static void PriceModel(int weekday)
        {
            // Every day has a different price, taking into account available tickets left
            int newTicketPrice = 0;
            int[] dayFee = { 80, 90, 100, 110, 120 };

            
            Park1.ChangePrice(newTicketPrice);
        }

        // Event handler for order in queue
        // Create an order processing thread
        public void OrderReceived(int orderIndex)
        {
            Order order = MultiCellBuffer.getOneCell(orderIndex);
            Thread process = new Thread(new ParameterizedThreadStart(OrderProcessing));
            process.Start(order);
            //check the order
            //After processing the order, put the thread to sleep to restock tickets
            
        }

        public void OrderProcessing(Object processOrder)
        {
            Order order = (Order)processOrder;
            Thread.Sleep(500);
            DateTime dateOne = order.getStartTime();
            DateTime dateTwo = DateTime.Now;
            TimeSpan diff = dateTwo.Subtract(dateOne);


            if (order.getCardNo() >= 5000 && order.getCardNo() <= 7000)
            {
                double totalAmount = order.getUnitPrice() * order.getAmount();
                Console.WriteLine("{0}: Order confirmed {1}", order.getSenderId(), diff);
            }
            else
            {
                Console.WriteLine("{0}: Invalid card number", order.getSenderId());
            }
            availableTickets -= order.getAmount();
        }
    }

    public class Park2
    {
        static Random rng = new Random();
        public static event priceCutEvent PriceCut; // Link event to delegate
        private static double ticketPrice = 80.0;
        private static int numCuts = 0;
        private static int currentDay = 0;
        private static int availableTickets = 300;

        public double GetPrice()
        {
            return ticketPrice;
        }

        public static void ChangePrice(double price)
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
                Int32 q = rng.Next(85000, 86556);
                // Console.WriteLine("New Price is {0}", p); 
                Park2.PriceModel(currentDay, r, q);
                currentDay++;
                currentDay %= 5;
            }
        }

        public static void PriceModel(int weekday, int numberOfTickets, int zipCode) //Need to have two price models for different parks<<<
        {
            double newprice = 80;
            int[] dayFee = {100, 120, 130, 140, 150 };

            Park2.ChangePrice(newprice);
        }

        // Event handler for order in queue
        // Create an order processing thread
        public void OrderReceived(int orderIndex)
        {
            Order order = MultiCellBuffer.getOneCell(orderIndex);
            Thread process = new Thread(new ParameterizedThreadStart(OrderProcessing));
            process.Start(order);
            //check the order
            //After processing the order, put the thread to sleep to restock tickets

        }

        public void OrderProcessing(Object processOrder)
        {
            Order order = (Order)processOrder;
            Thread.Sleep(500);
            DateTime dateOne = order.getStartTime();
            DateTime dateTwo = DateTime.Now;
            TimeSpan diff = dateTwo.Subtract(dateOne);
         

            if (order.getCardNo() >= 5000 && order.getCardNo() <= 7000)
            {
                double totalAmount = order.getUnitPrice() * order.getAmount();
                Console.WriteLine("{0}: Order confirmed {1}", order.getSenderId(), diff);
            }
            else
            {
                Console.WriteLine("{0}: Invalid card number", order.getSenderId());
            }
            availableTickets -= order.getAmount();
        }
    }

    public class TicketAgency 
    {
        static Random rng = new Random();
        private int stockOfTickets = 0;

        public void TicketAgencyFunc() 
        {
            // Each ticket agency has no ticket stock for each park in the beginning
            OrderTickets("Park 1", 300, 5228, 80);
            OrderTickets("Park 2", 300, 6228, 100);
        } 

        public void OrderTickets(String parkName, int numOfTickets, int cardNum, double unitPrice) 
        {
            DateTime localDate = DateTime.Now;
            Order newOrder = new Order();
            newOrder.setSenderId(Thread.CurrentThread.Name);
            newOrder.setReceiverId(parkName);
            newOrder.setAmount(numOfTickets);
            newOrder.setCardNo(cardNum);
            newOrder.setUnitPrice(unitPrice);
            newOrder.setStartTime(localDate);
            MultiCellBuffer.setOneCell(newOrder);
            
        }

        // Event handler for price cut
        public void TicketOnSale(String parkName, double unitPrice) 
        {
            // order tickets from Park – send order into queue 
            // Need to decide if we need to purchase more tickets depending on inventory
            int cardNum = rng.Next(4500, 7500);
            // If the ticket agency has less than 150 tickets then purchase more
            if (stockOfTickets < 150)
            {
               
            }
            else
            {
                // Decide if we want to purchase more
            }

            // profit = priceofticket + .5*priceofticket
            Console.WriteLine("{0}: {1} tickets are on sale as low as ${2} each", Thread.CurrentThread.Name, parkName, unitPrice);
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
            MultiCellBuffer.init(new orderEvent1(park1.OrderReceived), new orderEvent2(park2.OrderReceived));

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