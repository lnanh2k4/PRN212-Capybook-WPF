using Capybook.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;

namespace Capybook.ViewModels
{
    class OrderDetailVM : BaseVM
    {
        public ObservableCollection<OrderDetail> OrderDetails { get; set; }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private decimal _totalDiscount;
        public decimal TotalDiscount
        {
            get => _totalDiscount;
            set
            {
                _totalDiscount = value;
                OnPropertyChanged(nameof(TotalDiscount));
            }
        }

        private double? _voucherDiscount;

        public OrderDetailVM(int orderId)
        {
            OrderDetails = new ObservableCollection<OrderDetail>();
            OrderDetails.CollectionChanged += (s, e) => CalculateTotals();
            Load(orderId);
        }

        private void Load(int orderId)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var order = context.Orders
                    .Include(o => o.Vou)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                    .FirstOrDefault(o => o.OrderId == orderId);

                if (order != null)
                {
                    _voucherDiscount = order.Vou?.Discount;

                    OrderDetails.Clear();
                    foreach (var item in order.OrderDetails)
                    {
                        OrderDetails.Add(item);
                    }

                    CalculateTotals();
                }
            }
        }

        private void CalculateTotals()
        {
            decimal totalBeforeDiscount = OrderDetails.Sum(od => od.Book != null && od.Book.BookPrice.HasValue
                                                    ? od.Quantity.GetValueOrDefault() * od.Book.BookPrice.GetValueOrDefault()
                                                    : 0);
            TotalDiscount = _voucherDiscount.HasValue ? totalBeforeDiscount * (decimal)_voucherDiscount.Value / 100 : 0;

            TotalPrice = totalBeforeDiscount - TotalDiscount;
        }
    }
}
