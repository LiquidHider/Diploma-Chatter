import { AbstractControl, FormControl, ValidatorFn, Validators } from "@angular/forms";

export class CustomValidators{
   static matchValues(matchTo: string): ValidatorFn {
        return (control: AbstractControl) => {
          const value = control.parent?.get(matchTo)?.value;
          return control.value === value ? null : { ismatching: true };
        };
    }

    static noDigits(arg: any): ValidatorFn {
        return (control: AbstractControl) => {
          const value = control.value;
          
          const digitsCheck = /\d/g.test(value);
          return digitsCheck === false ? null :  { nodigits: true };
        };
      }
}