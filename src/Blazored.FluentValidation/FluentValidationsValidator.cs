﻿using FluentValidation;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;

namespace Blazored.FluentValidation
{
    public class FluentValidationValidator : ComponentBase
    {
        [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;

        [CascadingParameter] private EditContext? CurrentEditContext { get; set; }

        [Parameter] public IValidator? Validator { get; set; }
        [Parameter] public bool DisableAssemblyScanning { get; set; }

        internal Action<ValidationStrategy<object>>? Options;

        public bool Validate(Action<ValidationStrategy<object>> options)
        {
            if (CurrentEditContext is null)
            {
                throw new NullReferenceException(nameof(CurrentEditContext));
            }

            Options = options;

            try
            {
                return CurrentEditContext.Validate();
            }
            finally
            {
                Options = null;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(FluentValidationValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(FluentValidationValidator)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            await CurrentEditContext.AddFluentValidation(ServiceProvider, DisableAssemblyScanning, Validator, this);
            await base.OnInitializedAsync();
        }
    }
}
