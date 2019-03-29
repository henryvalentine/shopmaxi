using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class DataSynchServices
	{
        private readonly DataSynchRepository _dataSynchRepository;
        public DataSynchServices()
		{
            _dataSynchRepository = new DataSynchRepository();
		}

        public bool SynchData ()
        {
            return true;
            //return _dataSynchRepository.SyncEntities();
        }

	}

}
