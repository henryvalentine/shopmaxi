using System.Collections.Generic;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class TransferNoteServices
	{
        private readonly TransferNoteRepository _transferNoteRepository;  
        public TransferNoteServices()
		{
            _transferNoteRepository = new TransferNoteRepository();
		}

        public long AddTransferNote(TransferNoteObject transferNote, out string transferNoteNumber)
		{
            return _transferNoteRepository.AddTransferNote(transferNote, out transferNoteNumber);
		}

        public long DeleteTransferNoteItem(long orderItemId)
        {
            return _transferNoteRepository.DeleteTransferNoteItem(orderItemId);
        }

        public long DeleteTransferNote(long transferNoteId)
        {
            return _transferNoteRepository.DeleteTransferNote(transferNoteId);
        }

        public long UpdateTransferNote(TransferNoteObject transferNote)
		{
			return _transferNoteRepository.UpdateTransferNote(transferNote);
		}

        public long ConvertTransferNoteToInvoice(string transferNoteNumber)
        {
            return _transferNoteRepository.ConvertTransferNoteToInvoice(transferNoteNumber);
        }
        
        public TransferNoteObject GetTransferNote(long transferNoteId)
        {
            return _transferNoteRepository.GetTransferNote(transferNoteId);
        }

        public TransferNoteObject GetTransferNoteByRef(string refNumber)  
        {
            return _transferNoteRepository.GetTransferNoteByRef(refNumber);
        }
    
        public TransferNoteObject GetTransferNoteDetails(long transferNoteId)
        {
            return _transferNoteRepository.GetTransferNote(transferNoteId);
        }

        public List<TransferNoteObject> GetTransferNoteObjects(int? itemsPerPage, int? pageNumber, out int count)
        {
            return _transferNoteRepository.GetTransferNotes(itemsPerPage, pageNumber, out count);
        }
        public List<TransferNoteObject> GetTransferNotesByOutlet(int? itemsPerPage, int? pageNumber, out int count, int outletId)
        {
            return _transferNoteRepository.GetTransferNotesByOutlet(itemsPerPage, pageNumber, out count, outletId);
        }

        public List<TransferNoteObject> GetTransferNotesByEmployee(int? itemsPerPage, int? pageNumber, out int count, long employeeId)
        {
            return _transferNoteRepository.GetTransferNotesByEmployee(itemsPerPage, pageNumber, out count, employeeId);
        }

        public List<TransferNoteObject> SearchTransferNotes(string searchCriteria)
        {
            return _transferNoteRepository.SearchTransferNotes(searchCriteria);
        }

        public List<TransferNoteObject> SearchOutletTransferNote(string searchCriteria, int outletId)
        {
            return _transferNoteRepository.SearchOutletTransferNote(searchCriteria, outletId);
        }

        public List<TransferNoteObject> SearchEmployeeTransferNote(string searchCriteria, long employeeId)
        {
            return _transferNoteRepository.SearchEmployeeTransferNote(searchCriteria, employeeId);
        }

	}
    
}

