using System.ComponentModel.DataAnnotations;

namespace TravelExpertsGUI.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(25, ErrorMessage = "First name cannot be more than 25 characters")]
    public string? CustFirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(25, ErrorMessage = "Last name cannot be more than 25 characters")]
    public string? CustLastName { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [StringLength(75, ErrorMessage = "Address cannot be more than 75 characters")]
    public string? CustAddress { get; set; }

    [Required(ErrorMessage = "City is required")]
    [StringLength(50, ErrorMessage = "City cannot be more than 50 characters")]
    public string? CustCity { get; set; }

    [Required(ErrorMessage = "Province is required")]
    [StringLength(2, ErrorMessage = "Province cannot be more than 2 characters")]
    public string? CustProv { get; set; }

    [Required(ErrorMessage = "Postal code is required")]
    [StringLength(7, ErrorMessage = "Postal code cannot be more than 7 characters")]
    [RegularExpression(@"^[A-Z]\d[A-Z] ?\d[A-Z]\d$", ErrorMessage = "Postal code must be in the format A1A 1A1")]
    public string? CustPostal { get; set; }

    [Required(ErrorMessage = "Country is required")]
    [StringLength(25, ErrorMessage = "Country cannot be more than 25 characters")]
    public string? CustCountry { get; set; }

    [Required(ErrorMessage = "Home phone number is required")]
    [StringLength(20, ErrorMessage = "Home phone number cannot be more than 20 characters")]
    [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Phone number must be in the format XXX-XXX-XXXX.")]
    public string? CustHomePhone { get; set; }

    [StringLength(20, ErrorMessage = "Business phone number cannot be more than 20 characters")]
    [RegularExpression(@"^\d{3}-\d{3}-\d{4}$", ErrorMessage = "Phone number must be in the format XXX-XXX-XXXX.")]
    public string? CustBusPhone { get; set; }

    [StringLength(50, ErrorMessage = "Email cannot be more than 50 characters")]
    public string? CustEmail { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username cannot be more than 50 characters")]
    public string? CustUsername { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(50, ErrorMessage = "Password cannot be more than 50 characters")]
    public string? CustPassword { get; set; }

    //Confirm Password and compare with password
    [Required(ErrorMessage = "Confirm password is required")]
    [StringLength(50, ErrorMessage = "Confirm password cannot be more than 50 characters")]
    [Compare("CustPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }
}