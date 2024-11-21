# Documentation

## Liste des commandes pour intéragir avec le serveur TCP

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
    "entities": un tableau avec les ID des clients connectés
}
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
    "entities": un tableau avec les ID des clients connectés
}
```

#### Récupérer tous les clients connectés au serveur
```bash
getConnectedClientsIds
```

##### Valeur de retour: <br>
```bash
{
    "entities": un tableau avec les ID des clients connectés
}
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
    "entities": un tableau avec les ID des clients connectés
}
```
