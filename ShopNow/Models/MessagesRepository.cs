using ShopNow.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ShopNow.Models
{

    public class MessagesRepository
        {
            readonly string _connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //public IEnumerable<Messages> GetAllMessages()
        //{
        //    var messages = new List<Messages>();

        //    using (var connection = new SqlConnection(_connString))
        //        {
        //            connection.Open();
        //            using (var command = new SqlCommand(@"SELECT [MessageID], [Message], [EmptyMessage], [Date] FROM [dbo].[Messages]", connection))
        //            {
        //                command.Notification = null;

        //                var dependency = new SqlDependency(command);
        //                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

        //                if (connection.State == ConnectionState.Closed)
        //                    connection.Open();
        //            var reader = command.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                messages.Add(item: new Messages { MessageID = (int)reader["MessageID"], Message = (string)reader["Message"], EmptyMessage = reader["EmptyMessage"] != DBNull.Value ? (string)reader["EmptyMessage"] : "", MessageDate = Convert.ToDateTime(reader["Date"]) });
        //            }
        //        }

        //    }
        //    return messages;


        //}
        private sncEntities db = new sncEntities();
        public Object GetAllMessages()
        {
            int count = 0;
            var data = new {  NewOrder  = 0, Shopaccptence = "0" ,Deliveryaccept="0",shoppickup="0",customerdelivery="0",orderwithoutdeliveryboy="0"};
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                //using (var command = new SqlCommand(@"SELECT [MessageID], [Message], [EmptyMessage], [Date] FROM [dbo].[Messages]", connection))
                using (var command = new SqlCommand(@"SELECT top(1) Id FROM [dbo].[Orders] where Status in (2,3,4,5,8)", connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var lst = db.GetDashBoardDetails().ToList();
                        int? NewOrders = lst[0].NewOrder;
                       // data.NewOrder =Convert.ToInt32(lst[0].NewOrder.Value);
                     //  data =new { NewOrder = lst[0].NewOrder, Shopaccptence = lst[0].Shopacceptance, Deliveryaccept = lst[0].deliveryaccpetance, shoppickup = lst[0].shoppickup, customerdelivery = lst[0].customerdelivery, orderwithoutdeliveryboy = lst[0].orderwithoutdeliveryboy };


                    }
                }

            }
    
            return data;

        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
            {
                if (e.Type == SqlNotificationType.Change)
                {
                    MessagesHub.SendMessages();
                }
            }
        }
    
}