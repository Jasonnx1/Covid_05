﻿using BillingManagement.Models;
using BillingManagement.UI.ViewModels.Commands;
using Inventaire;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace BillingManagement.UI.ViewModels
{
    class MainViewModel : BaseViewModel
    {
		private BaseViewModel _vm;

		public BaseViewModel VM
		{
			get { return _vm; }
			set {
				_vm = value;
				OnPropertyChanged();
			}
		}




		private string searchCriteria;

		public string SearchCriteria
		{
			get { return searchCriteria; }
			set { 
				searchCriteria = value;
				OnPropertyChanged();
			}
		}



		public CustomerViewModel customerViewModel;
		public InvoiceViewModel invoiceViewModel;

		public ChangeViewCommand ChangeViewCommand { get; set; }

		public DelegateCommand<object> AddNewItemCommand { get; private set; }

		public BillingContext db { get; set; }

		public DelegateCommand<Invoice> DisplayInvoiceCommand { get; private set; }
		public DelegateCommand<Customer> DisplayCustomerCommand { get; private set; }

		public DelegateCommand<Customer> AddInvoiceToCustomerCommand { get; private set; }

		public RelayCommand<IClosable> ExitClickCommand { get; private set; }

		



		public MainViewModel()
		{
			ChangeViewCommand = new ChangeViewCommand(ChangeView);
			DisplayInvoiceCommand = new DelegateCommand<Invoice>(DisplayInvoice);
			DisplayCustomerCommand = new DelegateCommand<Customer>(DisplayCustomer);
			ExitClickCommand = new RelayCommand<IClosable>(Exit);

			AddNewItemCommand = new DelegateCommand<object>(AddNewItem, CanAddNewItem);
			AddInvoiceToCustomerCommand = new DelegateCommand<Customer>(AddInvoiceToCustomer);
			db = new BillingContext();
			
			seedData();

			customerViewModel = new CustomerViewModel( new ObservableCollection<Customer>(db.Customers.OrderBy(c => c.LastName)), db);
			invoiceViewModel = new InvoiceViewModel(new ObservableCollection<Invoice>(db.Invoices));

			VM = customerViewModel;

		}


		void seedData()
		{
			
		}



		private void ChangeView(string vm)
		{
			switch (vm)
			{
				case "customers":
					VM = customerViewModel;
					break;
				case "invoices":
					VM = invoiceViewModel;
					break;
			}
		}

		private void DisplayInvoice(Invoice invoice)
		{
			invoiceViewModel.SelectedInvoice = invoice;
			VM = invoiceViewModel;
		}

		private void DisplayCustomer(Customer customer)
		{
			customerViewModel.SelectedCustomer = customer;
			VM = customerViewModel;
		}

		private void AddInvoiceToCustomer(Customer c)
		{
			var invoice = new Invoice(c);
			c.Invoices.Add(invoice);
			DisplayInvoice(invoice);
		}

		private void AddNewItem (object item)
		{
			if (VM == customerViewModel)
			{
				var c = new Customer() { Name = "TBD", LastName = "TBD" };
				db.Customers.Add(c);
				db.SaveChanges();
				customerViewModel.Customers.Clear();
				customerViewModel.Customers = new ObservableCollection<Customer>(db.Customers.OrderBy(c => c.LastName));
				customerViewModel.SelectedCustomer = c;
			}
		}

		


		private bool CanAddNewItem(object o)
		{
			bool result = false;

			result = VM == customerViewModel;
			return result;
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Shutdown();
		}

		private void Exit(IClosable window)
		{
			if (window != null)
			{
				window.Close();
			}
		}


	}
}
