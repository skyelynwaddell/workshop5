using Microsoft.EntityFrameworkCore;

namespace TravelExpertsData;

public class BookingDetailManager
{
    public static (List<BookingDetail> BookingDetails, decimal TotalCost) GetDetailsByCustomer(TravelExpertsContext db,
        int? customerId)
    {
        List<BookingDetail> bookingDetails = db.BookingDetails
            .Include(bd => bd.Booking)
            .Include(bd => bd.Fee)
            .Include(bd => bd.Booking!.Package)
            .Where(bd => bd.Booking != null && bd.Booking.CustomerId == customerId)
            .OrderBy(bd => bd.Booking!.BookingDate).ToList();

        decimal totalCost = bookingDetails.Sum(bd => bd.BasePrice ?? 0) +
                            bookingDetails.Sum(bd => bd.Fee?.FeeAmt ?? 0) +
                            bookingDetails.Sum(bd => bd.AgencyCommission ?? 0);
        return (bookingDetails, totalCost);
    }

    public static decimal GetTotalCost(List<BookingDetail> details)
    {
        decimal total = 0;
        foreach (var detail in details)
        {
            total += detail.BasePrice ?? 0;
            if (detail.Fee != null)
            {
                total += detail.Fee.FeeAmt;
            }
        }

        return total;
    }
}