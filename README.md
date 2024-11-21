# Documentation

## Liste des commandes pour intéragir avec le serveur TCP

### Table des matières
- [Client](#client)
    - [Se connecter au server](#se-connecter-au-server)
    - [Se déconnecter du server](#se-déconnecter-du-server)
    - [Récupérer tous les clients connectés au serveur](#récupérer-tous-les-clients-connectés-au-serveur)
- [Bonus](#bonus)
    - [Récupérer tous les bonus du serveur](#récupérer-tous-les-bonus-du-serveur)
    - [Changer le isActive d'un bonus](#changer-le-isactive-dun-bonus)
- [Score](#score)
    - [Incrémenter le score d'un client](#incrémenter-le-score-dun-client)

### Client

#### Se connecter au server
```bash
connect clientId
```
##### Paramètres requis:
* `clientId` (string) : Identifiant unique du client qui souhaite se connecter au serveur.

##### Valeur de retour: <br>
```bash
{
    "entities": un tableau avec des EntityData
}

# EntityData est un objet composé comme ceci : 
# public class EntityData
# {
#     public string ID;         // Identifiant unique de l'entité
#     public Vector3 position;  // Position de l'entité dans l'espace (x, y, z)
#     public Vector3 rotation;  // Rotation de l'entité dans l'espace (x, y, z)
#     public int score;         // Score associé à l'entité
# }
```

#### Se déconnecter du server
```bash
disconnect clientId
```
##### Paramètres requis:
* `clientId` (string) : Identifiant unique du client qui souhaite se déconnecter du serveur.

##### Valeur de retour: <br>
```bash
{
    "entities": un tableau avec des EntityData
}

# EntityData est un objet composé comme ceci : 
# public class EntityData
# {
#     public string ID;         // Identifiant unique de l'entité
#     public Vector3 position;  // Position de l'entité dans l'espace (x, y, z)
#     public Vector3 rotation;  // Rotation de l'entité dans l'espace (x, y, z)
#     public int score;         // Score associé à l'entité
# }
```

#### Récupérer tous les clients connectés au serveur
```bash
getConnectedClients
```

##### Valeur de retour: <br>
```bash
{
    "entities": un tableau avec des EntityData
}

# EntityData est un objet composé comme ceci : 
# public class EntityData
# {
#     public string ID;         // Identifiant unique de l'entité
#     public Vector3 position;  // Position de l'entité dans l'espace (x, y, z)
#     public Vector3 rotation;  // Rotation de l'entité dans l'espace (x, y, z)
#     public int score;         // Score associé à l'entité
# }
```

### Bonus

#### Récupérer tous les bonus du serveur
```bash
getBonus
```

##### Valeur de retour: <br>
```bash
{
    "bonuses": un tableau avec des BonusData
}

# EntityData est un objet composé comme ceci : 
# public class BonusData
# {
#     public string ID;         // Identifiant unique du bonus
#     public Vector3 position;  // Position du bonus dans l'espace (x, y, z)
#     public bool isActive;  // Booléen qui dit si l'objet est actif ou pas
# }
```

#### Changer le isActive d'un bonus
```bash
updateBonus bonusId bool
```
##### Paramètres requis:
* `bonusId` (string) : Identifiant unique du bonus qui souhaite se faire modifier.
* `bool` (bool) : Nouvelle valeur souhaité pour le bonus.

##### Valeur de retour: <br>
```bash
{
    "bonuses": un tableau avec des BonusData
}

# EntityData est un objet composé comme ceci : 
# public class BonusData
# {
#     public string ID;         // Identifiant unique du bonus
#     public Vector3 position;  // Position du bonus dans l'espace (x, y, z)
#     public bool isActive;  // Booléen qui dit si l'objet est actif ou pas
# }
```

### Score

#### Incrémenter le score d'un client
```bash
incrementScore clientId
```

##### Paramètres requis:
* `clientId` (string) : Identifiant unique du client qui souhaite incrémenter son score.

##### Valeur de retour: <br>
```bash
{
    "entities": un tableau avec des EntityData
}

# EntityData est un objet composé comme ceci : 
# public class EntityData
# {
#     public string ID;         // Identifiant unique de l'entité
#     public Vector3 position;  // Position de l'entité dans l'espace (x, y, z)
#     public Vector3 rotation;  // Rotation de l'entité dans l'espace (x, y, z)
#     public int score;         // Score associé à l'entité
# }
```
