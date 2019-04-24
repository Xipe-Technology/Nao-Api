using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using XipeServices.Validations;

namespace XipeADNApp.Validations
{
    public class ValidatableObject<T> : BindableObject, IValidity
    {
        T mainValue;
        bool isValid;

        public List<IValidationRule<T>> Validations { get; }
        public List<IComparisonRule<T>> Comparison { get; }
        public ObservableCollection<string> Errors { get; }
        public string ErrorMessage { get; set; }
        public T Value
        {
            get => mainValue;
            set
            {
                mainValue = value;
                OnPropertyChanged();
            }
        }
        public bool IsValid
        {
            get => isValid;
            set
            {
                isValid = value;
                OnPropertyChanged();
            }
        }

        public ValidatableObject()
        {
            isValid = true;
            Errors = new ObservableCollection<string>();
            Validations = new List<IValidationRule<T>>();
            Comparison = new List<IComparisonRule<T>>();
        }

        public bool Validate()
        {
            Errors.Clear();
            IsValid = true;
            var errors = Validations.Where(v => !v.Check(Value))                        
                .Select(v => v.ValidationMessage).ToList();
            errors.AddRange(Comparison.Where(x => !x.Compare(Value)).Select(v => v.ValidationMessage));
            foreach (var error in errors)
                Errors.Add(error);
            if (Errors.Any()) 
            {
                IsValid = false;
                ErrorMessage = Errors.First();
            }
            return IsValid;
        }
    }
}
