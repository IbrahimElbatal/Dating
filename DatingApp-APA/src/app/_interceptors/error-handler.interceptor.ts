import { HttpInterceptor, HttpEvent, HttpHandler, HttpRequest, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NgModule } from '@angular/core';

export class ErrorHandler implements HttpInterceptor{
    intercept(req: HttpRequest<any>,next: HttpHandler)
            : Observable<HttpEvent<any>> {
        //handle 401
        return next.handle(req)
            .pipe(
                catchError((err:HttpErrorResponse) =>{
                    if(err.status == 401){
                        return throwError('UnAuthorized');
                    }
                    
                    //validation errors & server errors
                    let errorMessage = '';
                    let serverError= err.error; 

                    if(serverError.errors && typeof(serverError.errors) == 'object'){
                        for (const key in err.error.errors) {
                            if(err.error.errors[key])
                                errorMessage += err.error.errors[key]+'\n';
                        }
                    }

                    return throwError(errorMessage || err.error || 'Server error');
                })
            );
    }

}


export const ErrorHandlingProvder = {
    provide :HTTP_INTERCEPTORS,
    useClass :ErrorHandler,
    multi :true
}