﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TP2_final.Models;

namespace TP2_final.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private const string pathMedias = "s_medias.json", pathUtilisateurs = "s_utilisateurs.json", pathEvaluations = "s_evalutations.json", pathFavoris = "s_favoris.json";
        private static string pathDossierSerial = @$"{Environment.CurrentDirectory}\Donnees";

        private static Catalogue catalogue;
        private static CatalogueUtilisateur catalogueUtilisateur;
        private static CatalogueFavoris catalogueFavoris;
        private static FavorisMediaViewModel favMed;
        private bool isSerializationToDo = true;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;

            if (isSerializationToDo)
            {
                isSerializationToDo = false;
                catalogue = new Catalogue();
                catalogueUtilisateur = new CatalogueUtilisateur();
                catalogueFavoris = new CatalogueFavoris();

                catalogue.Ajouter(pathMedias, pathDossierSerial);
                catalogueUtilisateur.Ajouter(pathUtilisateurs, pathDossierSerial);
                catalogueFavoris.Ajouter(pathFavoris, pathDossierSerial);

                favMed = new FavorisMediaViewModel
                {
                    Favoris = catalogueFavoris.GetCatalogue(),
                    Medias = catalogue.GetCatalogue()
                };
            }
        }

        public IActionResult Deco()
        {
            TempData.Clear();
            return RedirectToAction("Index", "NonConnecte");
        }

        public IActionResult Index()
        {
            TempData.Keep("user_id");
            TempData.Keep("username");
            return catalogueUtilisateur.GetUtilisateurByPseudo((string)TempData["username"]) is null ? RedirectToAction("Index", "NonConnecte") : View(catalogue);
        }

        public IActionResult Favoris()
        {
            TempData.Keep("user_id");
            TempData.Keep("username");
            return catalogueUtilisateur.GetUtilisateurByPseudo((string)TempData["username"]) is null ? RedirectToAction("Index", "NonConnecte") : View(favMed);
        }

        public IActionResult Fiche(string nom)
        {
            TempData.Keep("user_id");
            TempData.Keep("username");
            ViewData["nomMedia"] = nom;
            return catalogueUtilisateur.GetUtilisateurByPseudo((string)TempData["username"]) is null ? RedirectToAction("Index", "NonConnecte") : View(catalogue);;
        }


        public IActionResult AjouterFavoris(string idMedia)
        {
            ViewData["idMedia"] = idMedia;
            Favoris fav = new Favoris(catalogueUtilisateur.GetUtilisateurByPseudo((string)TempData["username"]), catalogue.GetMedia(idMedia));
            catalogueFavoris.Ajouter(fav);
            catalogueFavoris.Sauvegarder(pathFavoris, pathDossierSerial);
            return RedirectToAction("Favoris");
            //return NoContent();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}