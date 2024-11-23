using RIS_SERVER.server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RIS_SERVER.src.common
{
    public static class DtoValidator
    {
        public static T Validate<T>(string json) where T : class
        {
            var dto = JsonSerializer.Deserialize<T>(json, Server.options);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

            if (isValid)
            {
                return dto;
            }

            List<string> errors = [];

            foreach (var error in validationResults)
            {
                errors.Add(error.ErrorMessage);
            }

            throw new WsException(400, $"Can't parse {typeof(T).Name}, following errors occured: \n{String.Join("\n", errors)}");
        }
    }
}
