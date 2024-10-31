using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Capybook.Utilities
{
    public class RoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int role = (int)value;
                if (role == 0)
                {
                    return "Admin";
                }
                else if (role == 1)
                {
                    return "Customer";
                }
                else if (role == 2)
                {
                    return "Seller Staff";
                }
                else if (role == 3)
                {
                    return "Warehouse Staff";
                }
            }
            return "Unknown";
         }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
