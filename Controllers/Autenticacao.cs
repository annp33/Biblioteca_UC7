using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using System.Linq;
using System.Collections.Generic; 

namespace Biblioteca.Controllers
{
    public class Autenticacao
    {
        public static void CheckLogin(Controller controller)
        {   
            if(string.IsNullOrEmpty(controller.HttpContext.Session.GetString("login")))
            {
                controller.Request.HttpContext.Response.Redirect("/Home/Login");
            }
        }

        //verifica se o usuário existe no banco de dados
        public static bool verificaLoginSenha(string login, string senha, Controller controller)
        {//define a controller, porque uma classe static não permite instanciação de objetos 
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                verificaSeUsuarioAdminExiste(bc);

                senha = Criptografia.TextoCriptografado(senha); // criptografa (transforma em hash) a senha que está entrando
                //o Hash não deixa a senha exposta no banco de dados

                //IQueryable é uma interface do C# para consultas, que armazena um objeto que não é uma lista
                IQueryable<Usuario> UsuarioEncontrado = bc.Usuarios.Where(u => u.Login == login && u.Senha == senha);

                //método Where() não retorna uma lista, então é preciso usar o IQueryable
                
                List<Usuario> ListaUsuarioEncontrado = UsuarioEncontrado.ToList(); // converte o objeto em lista
                
                if(ListaUsuarioEncontrado.Count==0){
                    return false;
                }else{
                    //adicionando na sessão da controller encaminhada, dado do usuário localizado
                    controller.HttpContext.Session.SetString("login", ListaUsuarioEncontrado[0].Login);
                    
                    controller.HttpContext.Session.SetString("nome", ListaUsuarioEncontrado[0].Nome);
                    
                    controller.HttpContext.Session.SetInt32("tipo", ListaUsuarioEncontrado[0].Tipo);
                    
                    return true;
                }
            }
        }

        public static void verificaSeUsuarioAdminExiste(BibliotecaContext bc)
        {
            IQueryable<Usuario> UsuarioEncontrado = bc.Usuarios.Where(u => u.Login =="admin");

            if(UsuarioEncontrado.ToList().Count == 0)
            {//Se o admin não existir (contagem de itens dentro do objeto UsuarioEncontrado for igual a zero), ele será criado por padrão com senha '123' 
                Usuario admin = new Usuario();
                admin.Nome = "Administrador";
                admin.Login = "admin";
                admin.Senha = Criptografia.TextoCriptografado("123");
                admin.Tipo = Usuario.ADMIN;

                bc.Usuarios.Add(admin);
                bc.SaveChanges();
            } 
        }

        public static void verificaSeUsuarioEAdmin(Controller controller)
        {   // recupera o dado "tipo" da sessão para verificar se o usuário logado é o admin
            // se a sessão não for do admin, ele é redirecionado
            if(!(controller.HttpContext.Session.GetInt32("tipo") == Usuario.ADMIN))
            {
                controller.Request.HttpContext.Response.Redirect("/Usuarios/NeedAdmin");
            }
        }
    }   
}