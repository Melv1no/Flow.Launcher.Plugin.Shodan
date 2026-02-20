# Flow.Launcher.Plugin.Shodan

Plugin Flow Launcher pour interagir avec l'API Shodan - le moteur de recherche pour les dispositifs connectés à Internet.

## 🚀 Installation

1. Téléchargez le plugin
2. Placez-le dans le dossier des plugins de Flow Launcher
3. Redémarrez Flow Launcher
4. Configurez votre clé API Shodan dans les paramètres du plugin

## 🔑 Configuration

Obtenez votre clé API gratuite sur [https://account.shodan.io/](https://account.shodan.io/)

## 📖 Commandes disponibles

### `shodan host <ip>`
Affiche les informations détaillées sur une adresse IP :
- Organisation
- Système d'exploitation
- Ports ouverts
- Localisation (pays, ville)
- Vulnérabilités détectées

**Exemple :** `shodan host 8.8.8.8`

### `shodan search <query>`
Recherche des dispositifs avec des filtres Shodan :
- Recherche par pays : `country:FR`
- Recherche par produit : `apache`
- Recherche par port : `port:80`

**Exemples :**
- `shodan search apache country:FR`
- `shodan search webcam port:8080`
- `shodan search nginx country:US city:Miami`

### `shodan dns <hostname>`
Résout un nom de domaine en adresse IP

**Exemple :** `shodan dns google.com`

### `shodan reverse <ip>`
Effectue un DNS inverse pour trouver le nom de domaine associé à une IP

**Exemple :** `shodan reverse 8.8.8.8`

### `shodan myip`
Affiche votre adresse IP publique

### `shodan info`
Affiche les informations de votre compte API :
- Plan actuel
- Crédits de recherche restants
- Crédits de scan restants

### `shodan help`
Affiche la liste de toutes les commandes disponibles

## 💡 Astuces

- Cliquez sur un résultat pour copier l'adresse IP dans le presse-papier
- Les recherches sont limitées à 10 résultats pour des performances optimales
- Utilisez les filtres Shodan pour des recherches plus précises

## 🔍 Filtres de recherche Shodan

Voici quelques filtres utiles :
- `country:FR` - Pays (code ISO)
- `city:"Paris"` - Ville
- `port:80` - Port spécifique
- `os:"Windows"` - Système d'exploitation
- `org:"Google"` - Organisation
- `product:"Apache"` - Produit/service
- `vuln:CVE-2014-0160` - Vulnérabilité spécifique


## 🔗 Liens utiles

- [Documentation API Shodan](https://developer.shodan.io/)
- [Filtres de recherche Shodan](https://www.shodan.io/search/filters)
- [Flow Launcher](https://www.flowlauncher.com/)
# Flow.Launcher.Plugin.Shodan
