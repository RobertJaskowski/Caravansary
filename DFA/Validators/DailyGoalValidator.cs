using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFA.Validators
{
    class DailyGoalValidator : AbstractValidator<string>
    {
        public DailyGoalValidator()
        {

            RuleFor(t => t)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Time is empty")
                .Must(BeTimeFormat).WithMessage("Input time incorrect format ex. \"1:30\" as of 1hour and 30minutes")
                .Must(BeLongerThanMinute).WithMessage("Your daily goal should be at least a minute!")
                .Must(BeLessThanADay).WithMessage("{}You are setting your daily goal for more than a day!");
        }

        private bool BeTimeFormat(string arg)
        {
            return TimeSpan.TryParse(arg, out TimeSpan t);
        }

        private bool BeLessThanADay(string arg)
        {
            return true;
            //arg.Days > 0;
        }

        private bool BeLongerThanMinute(string arg)
        {
            throw new NotImplementedException();
        }
    }
}
