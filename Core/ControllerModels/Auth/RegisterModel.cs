﻿using System.ComponentModel.DataAnnotations;

namespace Core.ControllerModels.Auth;

public class RegisterModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
}
