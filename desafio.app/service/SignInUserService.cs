using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Linq.Expressions;
using System.Threading.Tasks;
using desafio.app.context;
using desafio.app.domain;
using desafio.app.model;
using desafio.app;
using desafio.app.repository;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;

namespace desafio.app.service
{
    public class SignInService : AccountsService, ISignInService
    {
        private const string invalidUserAndPasswordError = "Usuário e/ou senha inválidos";
        
        public SignInService(IProfileRepository profileRepository, 
                             IUsersRepository usersRepository) : base(profileRepository, usersRepository)
        {
        }
        
        public RegisteredUserModel SignIn(SignInModel model){
           Assertion.IsFalse(string.IsNullOrEmpty(model.email), "Informe o e-mail do usuário.");
           Assertion.IsFalse(string.IsNullOrEmpty(model.senha), "Informe a senha do usuário.");
                
           user = usersRepository.GetByEmail(model.email);
                
           if(user==null)
               throw new InvalidUserException(invalidUserAndPasswordError);
                
           if(user.PasswordMatch(model.senha)){
                profile = profileRepository.GetByUserId(user.Id);
                user.SetLastLogon(DateTime.Now);
                user.SetUpdated(DateTime.Now);   
                GenerateUserToken();
                usersRepository.Update(user);
                    
                return GetRegisteredUserModel();
             }
             else
                throw new InvalidUserException(invalidUserAndPasswordError);
        }
    }
}