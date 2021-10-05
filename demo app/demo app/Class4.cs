using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace demo_app
{
    public class Class4
    {

        public static void try4()
        {
            Uri uri = new Uri("https://dev.azure.com/" + orgName);
			/// <param name="orgName">
			///     An organization in Azure DevOps Services. If you don't have one, you can create one for free:
			///     <see href="https://go.microsoft.com/fwlink/?LinkId=307137" />.
			/// </param>
			
            string personalAccessToken = personalAccessToken;
			/// <param name="personalAccessToken">
			///     A Personal Access Token, find out how to create one:
			///     <see href="https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops" />.
			/// </param>
			
			// create a wiql object and build our query
            Wiql wiql = new Wiql();

            wiql.Query = "SELECT [System.Id], [System.Title], [System.AssignedTo], [System.State], [Microsoft.VSTS.Scheduling.OriginalEstimate], [Microsoft.VSTS.Scheduling.RemainingWork] FROM WorkItems WHERE [System.WorkItemType] = 'Bug' AND [System.CreatedDate] <= @StartOfDay('-3d') AND [System.State] IN ('New', 'Ready For Testing', 'Active', 'In Testing', 'CheckedIn', 'Waiting on Dependency') AND [System.TeamProject]=' + project + ' ";
            var credentials = new VssBasicCredential(string.Empty, personalAccessToken);
           // var witClient = new WorkItemTrackingHttpClient(uri, credentials);


            VssConnection connection = null;
            connection = new VssConnection(uri, new VssBasicCredential(string.Empty, personalAccessToken));
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItemQueryResult tasks = witClient.QueryByWiqlAsync(wiql).Result;
            Console.WriteLine("Total Bugs Found : "+tasks.WorkItems.Count());
            Console.WriteLine("*********************************************");

            if (tasks.WorkItems.Any())
            {
                IEnumerable<WorkItemReference> tasksRefs;
                tasksRefs = tasks.WorkItems;
                foreach (var item in tasksRefs) {
                    //  IEnumerable<int> temp = item.Id;
                    List<int> ilist = new List<int> { item.Id };
                    IEnumerable<int> enumerable = ilist.AsEnumerable();
                    List<WorkItem> tasksList = witClient.GetWorkItemsAsync(enumerable).Result;

                    WorkItem workItem = witClient.GetWorkItemAsync(item.Id, null, null).Result;
                    Console.WriteLine("Id : " + workItem.Id);
                    foreach (var fields in workItem.Fields)
                    {

                        //Console.WriteLine(fields.Key + "   " + fields.Value);
                        if ( fields.Key == "System.Title" )
                        {
                            Console.WriteLine("Title : "+fields.Value);
                        }

                        if (fields.Key == "System.State")
                        {
                            Console.WriteLine("State : " + fields.Value);
                        }

                        if (fields.Key == "System.AssignedTo" )
                        {

                            dynamic d = fields.Value;
                           // dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(d);
                            Console.WriteLine("NAME : "  + d.GetType().GetProperty("DisplayName").GetValue(d, null));
                            Console.WriteLine("Account : " + d.GetType().GetProperty("UniqueName").GetValue(d, null));
                            //Console.WriteLine(fields.Value);
                        }

                        if (fields.Key == "System.IterationPath")
                        {
                            Console.WriteLine("Iteration Path : " + fields.Value);
                        }

                    }

                    Console.WriteLine("---------------------------------------------");
                }

            }

        }
    }


}
