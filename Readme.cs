# Nuny Food - Spécification Projet V1

## Présentation

Nuny Food est une plateforme web permettant aux membres de la diaspora togolaise de commander et payer des packs alimentaires destinés à leurs familles ou proches vivant au Togo.

L'objectif est d'éviter l'envoi direct d'argent et de garantir que l'aide soit utilisée pour l'achat de produits alimentaires.

---

# Stack Technique

## Backend

* ASP.NET Core Web API
* .NET 10
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* Swagger/OpenAPI
* Clean Architecture
* FluentValidation
* AutoMapper
* Serilog

## Frontend

* React 19
* TypeScript
* Vite
* React Router
* Axios
* Tailwind CSS
* Zustand
* TanStack Query
* React Hook Form
* Zod

---

# Acteurs

## Customer

Personne vivant à l'étranger qui commande des packs alimentaires.

## Beneficiary

Personne vivant au Togo qui reçoit les packs alimentaires.

## Administrator

Gère les produits, packs, commandes, paiements et livraisons.

## Delivery Agent

Effectue les livraisons des commandes.

---

# Fonctionnalités

## Authentification

Le client peut :

*Créer un compte
*Se connecter
* Se déconnecter
* Réinitialiser son mot de passe
* Modifier son profil

Sécurisation via JWT.

---

## Gestion des bénéficiaires

Le client peut :

*Ajouter un bénéficiaire
*Modifier un bénéficiaire
* Supprimer un bénéficiaire
* Consulter ses bénéficiaires

Règle métier :

Un bénéficiaire appartient à un seul client.

---

## Gestion des produits

L'administrateur peut :

* Ajouter un produit
* Modifier un produit
* Supprimer un produit
* Activer ou désactiver un produit
* Consulter la liste des produits

Exemples :

*Riz
* Huile
* Sucre
* Tomates
* Lait
* Pâtes
* Haricots

-- -

## Gestion des packs

Un pack est composé de plusieurs produits.

Exemples :

### Pack Étudiant

*Riz 5 kg
* Huile 1 L
* Sucre 2 kg

### Pack Famille

* Riz 25 kg
* Huile 5 L
* Sucre 5 kg
* Pâtes

### Pack Premium

* Produits variés

L'administrateur peut :

* Créer un pack
* Modifier un pack
* Supprimer un pack
* Activer ou désactiver un pack

---

## Gestion des commandes

Le client peut :

*Consulter les packs
*Visualiser le détail d'un pack
* Choisir un bénéficiaire
* Passer une commande

États possibles :

*Created
* PendingPayment
* Paid
* Preparing
* Assigned
* InDelivery
* Delivered
* Confirmed
* Cancelled

Toutes les modifications d'état doivent être historisées.

---

## Historique des statuts

Chaque changement d'état d'une commande est enregistré.

Exemple :

Commande #1001

* Created
*Paid
* Preparing
* Assigned
* Delivered
* Confirmed

-- -

## Paiements

Moyens de paiement prévus :

*PayPal
* TMoney
* Flooz

### Version 1

Les paiements réels ne sont pas encore intégrés.

Utiliser un FakePaymentProvider permettant de simuler :

*Paiement réussi
* Paiement échoué
* Paiement en attente

Prévoir une architecture permettant l'intégration future des APIs réelles sans refonte du système.

---

## Livraison

L'administrateur peut :

* Affecter une commande à un livreur

Le livreur peut :

*Consulter ses livraisons
*Marquer une livraison comme effectuée

Informations enregistrées :

*Nom du receveur
*Date de livraison
* Photo de preuve
* Signature
* Latitude GPS
* Longitude GPS

---

# Modèle de données

## Customer

* Id
* FirstName
* LastName
* Email
* PhoneNumber
* PasswordHash
* CreatedAt

## Beneficiary

* Id
* CustomerId
* FullName
* PhoneNumber
* Address
* City
* CreatedAt

## Product

* Id
* Name
* Description
* IsActive
* CreatedAt

## Pack

* Id
* Name
* Description
* Price
* ImageUrl
* IsActive
* CreatedAt

## PackProduct

* PackId
* ProductId
* Quantity

## Order

* Id
* CustomerId
* BeneficiaryId
* PackId
* Amount
* Status
* CreatedAt

## Payment

* Id
* OrderId
* Amount
* Method
* Status
* TransactionId
* CreatedAt

## OrderStatusHistory

* Id
* OrderId
* Status
* ChangedAt

## DeliveryAgent

* Id
* FullName
* PhoneNumber
* Zone
* IsActive

## Delivery

* Id
* OrderId
* DeliveryAgentId
* ReceiverName
* PhotoUrl
* SignatureUrl
* Latitude
* Longitude
* DeliveredAt

---

# Relations

Customer 1 -> N Beneficiaries

Customer 1 -> N Orders

Beneficiary 1 -> N Orders

Pack N -> N Products (via PackProduct)

Pack 1 -> N Orders

Order 1 -> N Payments

Order 1 -> N OrderStatusHistory

Order 1 -> 1 Delivery

DeliveryAgent 1 -> N Deliveries

---

# Architecture Backend

Solution :

NunyFood.sln

* NunyFood.Api
* NunyFood.Application
* NunyFood.Domain
* NunyFood.Infrastructure
* NunyFood.Tests

Respecter les principes de Clean Architecture.

---

# Architecture Frontend

Structure recommandée :

src /

*api /
*components /
*features /

  *auth /
  *beneficiaries /
  *products /
  *packs /
  *orders /
  *payments /
  *deliveries /
*layouts /
*pages /
*routes /
*store /
*hooks /
*types /
*utils /

---

# Pages Frontend

## Public

*Home
* Login
* Register

## Customer

* Dashboard
* Packs
* Pack Details
* Beneficiaries
*Orders
* Payment History
* Profile

## Administration

* Dashboard
* Products
* Packs
* Orders
* Payments
* Delivery Agents
* Deliveries
* Customers

---

# Design UI

Style moderne et professionnel.

Couleurs principales :

*Green : #16A34A
*Orange : #F97316
*White : #FFFFFF

Objectifs UX :

*Simplicité
* Rapidité
* Confiance
* Accessibilité
* Responsive Desktop, Tablet et Mobile

Interface inspirée des plateformes e-commerce modernes.

---

# Objectif Final

Développer une plateforme web complète permettant :

1.La gestion des produits alimentaires.
2. La gestion des packs alimentaires.
3. La gestion des bénéficiaires.
4. La création et le suivi des commandes.
5. La simulation puis l'intégration future des paiements.
6. La gestion des livraisons.
7. Un tableau de bord administrateur complet.
8. Usne architecture évolutive et maintenable basée sur .NET 10 et React TypeScript.
