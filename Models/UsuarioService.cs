using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Biblioteca.Models
{
    public class UsuarioService
    {
        public List<Usuario> Listar()
        {
            //abrindo a conexão com o banco de dados
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                return bc.Usuarios.ToList(); //obtém e retorna a lista com todos usuários
            }
        }

        public Usuario Listar(int id)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                return bc.Usuarios.Find(id); // obtém e retorna um usuário específico através do seu id 

                //pode ser usado o código abaixo também
                // bc.Usuarios.Where(u => u.Id == novoUser.Id).ToList();
                //onde u é a lista com todos os usuários
            }
        }

        public void incluirUsuario(Usuario novoUser)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                bc.Add(novoUser); //inclui usuário na lista
                bc.SaveChanges(); //salva o usuário na tabela do banco de dados
            }
        }

        public void editarUsuario(Usuario userEditar)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                Usuario u = bc.Usuarios.Find(userEditar.Id);

                //pode ser usado o código abaixo também
                //Usuario u = Listar(userEditar.Id)

                // o objeto u é a informação que está gravada no banco de dados e que ainda não foi alterada

                // especificação dos campos que podem ser alterados no banco de dados
                //troca a informação que está no banco pela alteração recebida pelo método 
                u.Login = userEditar.Login; 
                u.Nome = userEditar.Nome;
                if(u.Senha == userEditar.Senha){//só criptografa a senha se ela for modificada na edição
                    u.Senha = userEditar.Senha;
                }else{
                    u.Senha = Criptografia.TextoCriptografado(userEditar.Senha);
                }                
                u.Tipo = userEditar.Tipo;

                bc.SaveChanges(); //grava as alterações no banco de dados
            }
        }

        public void excluirUsuario(int id)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                bc.Usuarios.Remove(bc.Usuarios.Find(id));
                bc.SaveChanges(); 
                //o método remove precisa receber um objeto, por isso não funciona se passar somente o id como parâmetro

                //pode ser usado o código abaixo também
                //bc.Usuarios.Remove(Listar(id));

                // o código abaixo, usando uma variável auxilar, não é recomendado!
                // Usuario objUsuario = bc.Usuarios.Find(id);
                //bc.Usuarios.Remove(objUsuario);
            }
        }
    }
}