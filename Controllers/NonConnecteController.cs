﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TP2_final.Models;
using System.Text.RegularExpressions;

namespace TP2_final.Controllers
{
    public class NonConnecteController : Controller
    {
        private readonly ILogger<NonConnecteController> _logger;

        private const string pathMedias = "s_medias.json", pathUtilisateurs = "s_utilisateurs.json", pathEvaluations = "s_evalutations.json", pathFavoris = "s_favoris.json";
        private static string pathDossierSerial = @$"{Environment.CurrentDirectory}\Donnees";

        private CatalogueUtilisateur catalogueUtilisateur;
        private bool isSerializationToDo = true;

        public NonConnecteController(ILogger<NonConnecteController> logger)
        {
            _logger = logger;
            if (isSerializationToDo)
            {
                isSerializationToDo = false;
                catalogueUtilisateur = new CatalogueUtilisateur();

                catalogueUtilisateur.Ajouter(pathUtilisateurs, pathDossierSerial);
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        private static string validationPseudo(String pseudo) {//TODO
            if (pseudo.Length >= 5 && pseudo.Length <= 50 && new Regex("[a-z]", RegexOptions.IgnoreCase).IsMatch(pseudo) && new Regex("[0-9]+").IsMatch(pseudo) && !new Regex("[^a-zA-Z0-9]+").IsMatch(pseudo))
            return pseudo;
            else throw new Exception("hi");

        }

        private static string validationPassword(String pw) {//TODO

            return pw;   
        }

        private static string validationPrenom(String prenom) {//TODO

            return prenom;
        }

        private static string validationNom(String nom) {//TODO

            return nom; 
        }

        [HttpPost]
        public IActionResult Connecte()
        {
            string pseudo = validationPseudo(Request.Form["connPseudo"]);
            string mdp = validationPassword(Request.Form["connMdp"]);

            Utilisateur user = catalogueUtilisateur.GetUtilisateurByPseudo(pseudo);
            if (user is null || user.MotDePasse != mdp)
            {
                Console.WriteLine($"ERREUR DE CONN, user existe pas:{user is null} | mauvais pw: {(user?.MotDePasse != mdp)}");
                return View("MARCHE PAS, JAR SA EXPLOSE C SUR");
            }
            else
            {
                Console.WriteLine($"CONN pseudo:{pseudo}, mdp:{mdp}, id:{user.getId()}");
                TempData.Clear();
                TempData["user_id"] = user.getId();
                TempData["username"] = user.Pseudo;
                TempData.Keep();

                return RedirectToAction("Index", user.Role.ToString().ToLower(), catalogueUtilisateur);
            }
        }

        [HttpPost]
        public IActionResult Inscrire()
        {
            // TODO CHECK IF ALL INPUTS ARE GOOD
            // TODO CHECK IF USER ALREADY EXISTS
            if (false/*TODO*/)
            {
                Console.WriteLine($"TODO, ERREUR INSCRIPTION, inputs invalides:{"'TODO'"}, user existe deja:{"'TODO'"}");
            }
            else
            {
                Utilisateur newUser = new Utilisateur(Request.Form["insPseudo"], Request.Form["insMdp"], Request.Form["insNomFamille"], Request.Form["insPrenom"], Utilisateur.ROLE_DEFAULT);
                //chek si le user existe pas
                String pseudo = Request.Form["insPseudo"];

                Utilisateur user = catalogueUtilisateur.GetUtilisateurByPseudo(pseudo);
                if (user is not null)
                {
                    Console.WriteLine($"ERREUR DE CONN, user existe déjà:{user is not null}");
                    return View("Index");
                }
                else
                {
                    // TODO SERIALISATION
                    //TODO CHECK IF DESERIALISATION EST PAS CHIÉE PAR LES STATICS _x
                    Console.WriteLine($"no verification, New user: {newUser.ToString()}");

                    TempData.Clear();
                    TempData["user_id"] = newUser.getId();
                    TempData["username"] = newUser.Pseudo;
                    TempData.Keep();

                    return RedirectToAction("Index", newUser.Role.ToString().ToLower(), catalogueUtilisateur);
                }
            }
        }

        public IActionResult VuePartielleErreur()
        {
            return View("VuePartielleErreur");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}