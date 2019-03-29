using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class BulkCustomerRepository
    {
       private readonly IShopkeeperRepository<Customer> _repository;
       private readonly UnitOfWork _uoWork;
       private readonly ShopKeeperStoreEntities _db;

       public BulkCustomerRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
		}

        public List<GenericValidator> ReadExcelData(string filePath, string sheetName)
        {
            var feedBackList = new List<GenericValidator>();
            var gVal = new GenericValidator();

            if (filePath.Length < 3 || new FileInfo(filePath).Exists == false || (Path.GetExtension(filePath) != ".xls" && Path.GetExtension(filePath) != ".xlsx"))
            {
                gVal.Code = -1;
                gVal.Error = "Invalid Excel File Format";
                feedBackList.Add(gVal);
                return feedBackList;
            }

            if (sheetName.Length < 1)
            {
                gVal.Code = -1;
                gVal.Error = "Invalid Excel Sheet Name";
                feedBackList.Add(gVal);
                return feedBackList;
            }

            var connectionstring = string.Empty;
            switch (Path.GetExtension(filePath))
            {
                case ".xls":
                    connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES;'";
                    break;
                case ".xlsx":
                    connectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;ImportMixedTypes=Text'";
                    break;
            }

            if (connectionstring == "")
            {
                gVal.Code = -1;
                gVal.Error = "Process Error! Please try again later";
                feedBackList.Add(gVal);
                return feedBackList;
            }

            var selectString = @"SELECT [Last_Name],[Other_Names],[Email],[Mobile_Number],[Gender],[Outlet],[Customer_Type(eg: Retail Customer)],[Country_Name],[State_Name],[City_Name],[Street_Address] FROM [" + sheetName + "$]";
            var myCon = new OleDbConnection(connectionstring);
            try
            {
                if (myCon.State == ConnectionState.Closed)
                {
                    myCon.Open();
                }
                var cmd = new OleDbCommand(selectString, myCon);
                var adap = new OleDbDataAdapter(cmd);
                var ds = new DataSet();
                adap.Fill(ds);
                if (ds.Tables.Count < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invalid customer Template!";
                    feedBackList.Add(gVal);
                    return feedBackList;
                }

                var dv = new DataView(ds.Tables[0]);
                if (dv.Count < 1)
                {
                    gVal.Code = -1;
                    gVal.Error = "Invalid customer Template!";
                    feedBackList.Add(gVal);
                    return feedBackList;
                }
               
                for (var i = 0; i < dv.Count; i++)
                {
                    var lastName = dv[i].Row["Last_Name"].ToString().Trim();
                    if (string.IsNullOrEmpty(lastName.Trim())) { continue; }
                    var mInfo = ProcessRecord(dv[i]);
                    if (mInfo.Code < 1)
                    {
                        feedBackList.Add(mInfo);
                        continue;
                    }
                    feedBackList.Add(mInfo);
                }

                myCon.Close();
                return feedBackList;
            }
            catch (Exception ex)
            {
               myCon.Close();
                gVal.Code = -1;
                gVal.Error = ex.Message;
                feedBackList.Add(gVal);
                return feedBackList;
            }
        }

        private GenericValidator ProcessRecord(DataRowView dv)
        {
            var gVal = new GenericValidator();

            if (dv == null)
            {
                gVal.Code = -1;
                gVal.Error = "An unknown error was encountered.";
                return gVal;
            }
            try
            {
                using (var db = _db)
                {
                    var mInfo = new UserProfile
                    {
                        LastName = dv.Row["Last_Name"].ToString().Trim(),
                        IsActive = true
                    };

                    var outletName = dv.Row["Outlet"].ToString().Trim();
                    if (string.IsNullOrEmpty(outletName))
                    {
                        gVal.Error = "Please provide outlet.";
                        gVal.Code = -1;
                        return gVal; 
                    }

                    var outlets = db.StoreOutlets.Where(o => o.OutletName.Trim().ToLower() == outletName.ToLower().Trim()).ToList();
                    if (!outlets.Any())
                    {
                        gVal.Error = "Outlet information could not be found.";
                         gVal.Code = -1;
                        return gVal; 
                    }

                    var outlet = outlets[0];

                    var otherNames = dv.Row["Other_Names"].ToString();
                    if (string.IsNullOrEmpty(otherNames))
                    {
                        gVal.Error = "Please provide other names.";
                        gVal.Code = -1;
                        return gVal; 
                    }

                    mInfo.OtherNames = otherNames;
                    
                    var email = dv.Row["Email"].ToString().Trim();

                    if (!string.IsNullOrEmpty(email))
                    {
                        mInfo.ContactEmail = email;
                    }
                    
                    var mobilNumber = dv.Row["Mobile_Number"].ToString().Trim();

                    if (string.IsNullOrEmpty(mobilNumber))
                    {
                        gVal.Error = "Please provide customer's phone number";
                        gVal.Code = -1;
                        return gVal; 
                    }

                    mInfo.MobileNumber = mobilNumber;

                    var cInfo = new Customer
                    {
                        StoreOutletId = outlet.StoreOutletId
                    };

                    var gender = dv.Row["Gender"].ToString().Trim();

                    var customerType = dv.Row["Customer_Type(eg: Retail Customer)"].ToString().Trim();

                    if (string.IsNullOrEmpty(customerType))
                    {
                        gVal.Error = "Please provide customer type";
                         gVal.Code = -1;
                        return gVal; 
                    }

                    var types = db.StoreCustomerTypes.Where(c => c.Name.ToLower().Trim() == customerType.ToLower().Trim()).ToList();
                    if (types.Any())
                    {
                        cInfo.StoreCustomerTypeId = types[0].StoreCustomerTypeId;
                    }
                    else
                    {
                        var newCustomerType = new StoreCustomerType
                        {
                            Name = customerType,
                            Code = customerType,
                            CreditWorthy = false,
                            CreditLimit = 0
                        };

                        var processedType = db.StoreCustomerTypes.Add(newCustomerType);
                        db.SaveChanges();
                        cInfo.StoreCustomerTypeId = processedType.StoreCustomerTypeId;
                    }

                    var duplicates = db.UserProfiles.Count(m => m.MobileNumber.Trim() == mInfo.MobileNumber.Trim());
                    if (duplicates > 0)
                    {
                        gVal.Error = "Customer already Exists.";
                        gVal.Code = -1;
                        return gVal; 
                    }

                    var newAddress = new DeliveryAddress
                    {
                        MobileNumber = mobilNumber,
                        ContactEmail = email
                    };

                    var countryName = dv.Row["Country_Name"].ToString().Trim();

                    if (string.IsNullOrEmpty(countryName))
                    {
                        gVal.Error = "Please provide Country";
                         gVal.Code = -1;
                        return gVal; 
                    }

                    long countryId = 0;
                    var countries = db.StoreCountries.Where(c => c.Name.ToLower().Trim() == countryName.ToLower().Trim()).ToList();
                    if (countries.Any())
                    {
                        countryId = countries[0].StoreCountryId;
                    }
                    else
                    {
                        var newCountry = new StoreCountry
                        {
                            Name = countryName
                        };

                        var processedCountry = db.StoreCountries.Add(newCountry);
                        db.SaveChanges();
                        countryId = processedCountry.StoreCountryId;
                    }
                    
                    var stateName = dv.Row["State_Name"].ToString().Trim();

                    if (string.IsNullOrEmpty(stateName))
                    {
                        gVal.Error = "Please provide State";
                         gVal.Code = -1;
                        return gVal; 
                    }

                    long stateId = 0;

                    var states = db.StoreStates.Where(c => c.Name.ToLower().Trim() == stateName.ToLower().Trim()).ToList();
                    if (states.Any())
                    {
                        stateId = states[0].StoreStateId;
                    }
                    else
                    {
                        var newState = new StoreState
                        {
                            Name = stateName,
                            StoreCountryId = countryId
                        };

                        var processedState = db.StoreStates.Add(newState);
                        db.SaveChanges();
                        stateId = processedState.StoreStateId;
                    }

                    var cityName = dv.Row["City_Name"].ToString().Trim();

                    if (string.IsNullOrEmpty(cityName))
                    {
                        gVal.Error = "Please provide City";
                        gVal.Code = -1;
                        return gVal; 
                    }

                    var cities = db.StoreCities.Where(c => c.Name.ToLower().Trim() == cityName.ToLower().Trim()).ToList();
                    if (cities.Any())
                    {
                        newAddress.CityId = cities[0].StoreCityId;
                    }
                    else
                    {
                        var newCity = new StoreCity
                        {
                            Name = cityName,
                            StoreStateId = stateId
                        };

                        var processedCity = db.StoreCities.Add(newCity);
                        db.SaveChanges();
                        newAddress.CityId = processedCity.StoreCityId;
                    }

                    var address = dv.Row["Street_Address"].ToString().Trim();
                    if (string.IsNullOrEmpty(address))
                    {
                        gVal.Error = "Please provide Customer Address";
                        gVal.Code = -1;
                        return gVal; 
                    }
                    newAddress.AddressLine1 = address;

                    if (!string.IsNullOrEmpty(gender))
                    {
                        mInfo.Gender = gender;
                    }

                    var processedUser = db.UserProfiles.Add(mInfo);
                    db.SaveChanges();

                    cInfo.UserId = processedUser.Id;
                    var processedCustomer = db.Customers.Add(cInfo);
                    db.SaveChanges();

                    
                    newAddress.CustomerId = processedCustomer.CustomerId;
                    db.DeliveryAddresses.Add(newAddress);
                    db.SaveChanges();

                    gVal.Error = "Customer information was successfully processed";
                    gVal.Code = processedUser.Id;
                    return gVal; 
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                gVal.Code = -1;
                gVal.Error = "An unknown error was encountered.";
                return gVal;
            }

        }
    }

}
