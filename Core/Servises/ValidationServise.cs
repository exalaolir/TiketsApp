using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TiketsApp.Models;
using TiketsApp.res;
using TiketsApp.ViewModels.RegistrationVM;
using ValidatorResult = (bool result, string property, System.Collections.Generic.IReadOnlyList<string>? message);

namespace TiketsApp.Core.Servises
{
    internal sealed class ValidationServise
    {
        internal static (bool, IReadOnlyList<string>?) ValidateProperty<T> ( object property, T validatedObject, string propertyName )
        {
            if (validatedObject == null)
                throw new ArgumentNullException(nameof(validatedObject), "Передан  null");

            var context = new ValidationContext(validatedObject)
            {
                MemberName = propertyName
            };

            var validationResults = new List<ValidationResult>();


            bool noErrors = Validator.TryValidateProperty(property, context, validationResults);

            var errors = validationResults
                .Where(r => !string.IsNullOrEmpty(r.ErrorMessage))
                .Select(r => r.ErrorMessage!).ToList() as IReadOnlyList<string>;

            return (noErrors, errors);
        }

        internal static async Task<List<ValidatorResult>> ValidateOnRegister<T>(T newValue, int? id = null) where T : Role
        {
            List<ValidatorResult> results = [];
            using AppContext dbContext = new();


            await foreach( var user in dbContext.AllRoles.AsNoTracking().AsAsyncEnumerable())
            {
                if(user.Id == id) continue;

                if(newValue.Email == user.Email)
                {
                    results.Add((
                        result: false,
                        property: nameof(newValue.Email),
                        message: Consts.EmailMessage
                        ));
                } 

                if(typeof(T) == typeof(Saller) 
                    && user is Saller saller 
                    && newValue is Saller newSaller
                    && newSaller.Number == saller.Number)
                {
                    results.Add((
                       result: false,
                       property: "Num",
                       message: Consts.SallerIdMessage
                       ));
                }
            }

            return results;
        }

        internal static async Task<(Role?, List<ValidatorResult>)> ValidateOnLogin ( string email, string pass ) 
        {
            List<ValidatorResult> results = [];
            using AppContext dbContext = new();

            var user = await dbContext.AllRoles
                .Where(r =>  r.Email == email)
                .FirstOrDefaultAsync();
            
            if( user == null)
            {
                results.Add((
                       result: false,
                       property: nameof(LoginVM.Email),
                       message: Consts.EmailNotFoundMsg
                       ));
            }
            else if(!Hasher.VerifyPassword(pass, user.Password))
            {
                results.Add((
                      result: false,
                      property: nameof(LoginVM.Password),
                      message: Consts.PassNotFoundMsg
                      ));
                user = null;
            }

            if( user != null && (bool)user.BannedByAdmin!)
            {
                results.Add((
                     result: false,
                     property: nameof(LoginVM.Email),
                     message: Consts.BannedMsg
                     ));
                user = null;
            }

            return (user, results);
        }
    }
}
