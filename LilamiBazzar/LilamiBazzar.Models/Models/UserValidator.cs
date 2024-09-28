using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class UserValidator : AbstractValidator<User>
    {
        
            public UserValidator()
            {
            RuleFor(user => user.FullName)
                 .Matches(@"^[a-zA-Z]+$").WithMessage("FullName name can only letters")
                 .NotEmpty().WithMessage("FullName is required.")
                 .Length(1, 50).WithMessage("FullName must be between 1 and 50 characters long.");

            /*RuleFor(user => user.LastName)
                .Matches(@"^[a-zA-Z]+$").WithMessage("First name can only letters")
                .NotEmpty().WithMessage("Last name is required.")
                .Length(1, 50).WithMessage("Last name must be between 1 and 50 characters long.");*/

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format")
                .Length(0, 100).WithMessage("Email msut be 100 characters or less.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"[!@#$%^&*(),.?"":{}|<>]").WithMessage("Password must contain at least one special character.");

        
        }
    }
}
