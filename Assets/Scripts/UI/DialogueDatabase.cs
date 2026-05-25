using System.Collections.Generic;
using UnityEngine;

public enum LevelID
{
    Level1_Zest,
    Level2_Bastien,
    Level3_EveryLow,
    Level4_Nyctagin,
    Level5_Rend,
    Level6_Tonicity
}

public static class DialogueDatabase
{
    private static DialogueLine Line(string speaker, string text, bool isProtagonist)
    {
        return new DialogueLine { speakerName = speaker, text = text, isProtagonist = isProtagonist };
    }

    private static DialogueSequence Seq(params DialogueLine[] lines)
    {
        return new DialogueSequence { lines = new List<DialogueLine>(lines) };
    }

    public static DialogueSequence GetIntro(LevelID level)
    {
        switch (level)
        {
            case LevelID.Level1_Zest:
                return Seq(
                    Line("NARRADOR", "Bar Zestia. Un rincón tranquilo en el corazón de la ciudad. Aquí las noches son suaves y el ron siempre está frío. Es el lugar de Zest… y hoy, también, el punto de partida de Mixel.", false),
                    Line("MIXEL", "(frotándose la sien) Todavía me duele la cabeza. Los médicos dicen que fue el golpe, que estoy bien, pero… hay algo que no puedo recordar. Algo importante.", true),
                    Line("ZEST", "Lo sé. Me lo ibas a mostrar esa noche. Ibas corriendo hacia acá cuando te cayó ese ladrillo en la cabeza. Debió haber sido algo muy especial para salir así, sin ni siquiera avisar.", false),
                    Line("MIXEL", "Era… era perfecto, Zest. No sé explicarlo de otra forma. Era la mejor cosa que había creado en mi vida. Y ahora no queda nada. Solo un espacio en blanco donde debería estar la receta.", true),
                    Line("ZEST", "Entonces la reconstruyes. Pedazo por pedazo, trago por trago. Hay bartenders por toda la ciudad que llevan años perfeccionando su arte. Cada uno tiene algo que enseñarte, aunque no lo hagan de buen grado.", false),
                    Line("MIXEL", "¿Y creen que me van a dar sus recetas así nomás?", true),
                    Line("ZEST", "Para nada. Vas a tener que ganártelas. Pero antes de eso, te recuerdo cómo funciona todo esto. No me vas a ganar a mí en mi propio bar, pero algo aprenderás en el intento.", false),
                    Line("MIXEL", "Está bien. Enséñame.", true)
                );
            case LevelID.Level2_Bastien:
                return Seq(
                    Line("NARRADOR", "Thorne & Oak. Un bar que lleva décadas en el mismo lugar, con las mismas reglas y los mismos tragos. Aquí la tradición no es solo una costumbre, es una ley. Y los extraños no son bienvenidos.", false),
                    Line("MIXEL", "(en voz interna) Zest tenía razón. Este lugar me mira como si yo fuera el problema.", true),
                    Line("BASTIEN", "Llegas con mucho descaro para ser alguien que no conozco.", false),
                    Line("MIXEL", "No vine a molestar. Vine a aprender.", true),
                    Line("BASTIEN", "¿A aprender? En este bar no hay clases. Hay estándares. Y tú, muchacho, todavía no has demostrado nada.", false),
                    Line("MIXEL", "Entonces dame la oportunidad de hacerlo.", true),
                    Line("BASTIEN", "Las recetas de esta casa no se comparten. Se ganan. Y nadie que haya entrado aquí con esa mirada tuya ha salido con algo en las manos. Pero si insistes… te concedo ese honor.", false)
                );
            case LevelID.Level3_EveryLow:
                return Seq(
                    Line("NARRADOR", "Cyanara. Un bar que parece sacado de las profundidades de un lago. Hermoso a primera vista, pero con algo que no termina de calzar. Algo que se siente pesado, como el agua antes de una tormenta.", false),
                    Line("MIXEL", "(en voz interna) Visualmente impactante. Pero hay algo en el ambiente que no me convence. Como si todo aquí tuviera una segunda capa que no puedes ver a simple vista.", true),
                    Line("EVERY LOW", "Mira quién llegó. Un bartender errante con cara de turista. Déjame adivinar: alguien te mandó aquí buscando algo que no tienes. ¿Verdad?", false),
                    Line("MIXEL", "Vine por mi cuenta. Quiero conocer tu trago.", true),
                    Line("EVERY LOW", "Por supuesto que sí. Todo el mundo quiere algo de mí. El problema es que muy pocos llegan a merecerlo. Pero está bien. Disfrutemos del proceso.", false)
                );
            case LevelID.Level4_Nyctagin:
                return Seq(
                    Line("NARRADOR", "The Blooming Veil. Un bar que vive entre lo real y lo que no lo es. Aquí todo es bello, todo es vibrante, y nada es exactamente lo que parece.", false),
                    Line("MIXEL", "(en voz interna) Es un lugar completamente diferente a los anteriores. Menos rígido, más vivo. Pero siento que si bajo la guardia aquí, me pierdo de verdad.", true),
                    Line("NYCTAGIN", "Así que llegaste. Los demás en el bar me dijeron que venías, pero nunca sé si creerles. Exageran todo. ¿Qué es lo que buscas exactamente?", false),
                    Line("MIXEL", "Tu receta. Tu trago insignia.", true),
                    Line("NYCTAGIN", "Directo. Me gusta eso. La mayoría llega con rodeos, con cuentos, con pretextos. Te diré lo mismo que les digo a ellos: el conocimiento no debería estar atrapado en una sola mente. Pero tampoco lo regalo a cualquiera. Así que si lo quieres, tendrás que convencerme de que lo mereces.", false)
                );
            case LevelID.Level5_Rend:
                return Seq(
                    Line("NARRADOR", "Glass & Grind. Aquí no hay reglas, no hay estructura, no hay orden. Solo energía pura sin dirección. El tipo de lugar donde cualquier cosa puede pasar… y probablemente pase.", false),
                    Line("MIXEL", "(en voz interna) Nyctagin me advirtió. Pero esto es… otro nivel. No hay nada predecible aquí.", true),
                    Line("REND", "¡Finalmente alguien que llegó hasta acá sin rendirse en el camino! ¿Sabes cuánto tiempo llevaba esperando eso? Nyctagin me habló de ti. Dijo que eras interesante. Y cuando ella dice eso, generalmente tiene razón.", false),
                    Line("MIXEL", "Vine por tu trago. Y por información sobre el bar de la entrada de la zona.", true),
                    Line("REND", "¡Directo al punto! Me encanta. Mira, lo del trago está bien, lo de la información también, ¿pero sabes qué me interesa a mí antes de todo eso? La pelea. ¿Empezamos?", false)
                );
            case LevelID.Level6_Tonicity:
                return Seq(
                    Line("NARRADOR", "Cinderhall. Al entrar usando el pase que le entregó Rend, comprende de inmediato a qué se refería con la niebla. El lugar entero está cubierto por una bruma densa que se acumula en el suelo y se extiende como una marea lenta, obligando a iluminar el camino para poder avanzar, la visibilidad es limitada, y cada paso parece hundirse en un ambiente pesado y opresivo.", false),
                    Line("NARRADOR", "El bar tiene una estética sombría, casi gótica, las estructuras parecen más antiguas de lo que deberían, las luces son escasas y débiles, y todo el entorno transmite una sensación constante de incomodidad, el aire es denso, como si el lugar mismo respirara lentamente. Los habitantes observan a Mixel con frialdad, evaluándolo desde la distancia, sin disimular su desconfianza, cada mirada pesa.", false),
                    Line("NARRADOR", "Tras atravesar el bar, Mixel finalmente llega a la sala principal. Antes de entrar, se encuentra con una mujer de actitud tranquila, casi ajena a la oscuridad del lugar. No destaca por algo evidente, pero su presencia resulta curiosamente amable en medio de todo ese ambiente hostil.", false),
                    Line("TONIC", "Hola. Soy Tonic. Soy nueva en la ciudad. He viajado por distintos lugares aprendiendo nuevas técnicas de coctelería, y pronto inauguraré mi propio bar. Estás invitado a la apertura, pero solo si logras demostrar tu nivel, ya que el acceso estará reservado para quienes alcancen un alto rango en el circuito.", false),
                    Line("NARRADOR", "Tras esa breve charla, se despiden y el camino se abre hacia Tonicity.", false),
                    Line("TONIC", "Sabía que llegarías. Me alegra. ¿Estás listo?", false),
                    Line("MIXEL", "Llevo listo desde que empecé esto.", true)
                );
            default:
                return null;
        }
    }

    public static DialogueSequence GetOutro(LevelID level)
    {
        switch (level)
        {
            case LevelID.Level1_Zest:
                return Seq(
                    Line("ZEST", "Bien hecho. No tan mal para alguien con el cráneo recién pegado.", false),
                    Line("MIXEL", "*Toma el vaso. Lo observa un momento. Lo lleva a los labios.* Es… limpio. Ácido, pero equilibrado. ¿Qué es esto?", true),
                    Line("ZEST", "Un Daiquiri. Ron, jugo de limón, azúcar. Simple, pero si lo haces bien, es infalible. Es mi insignia. Aquí está la receta. Guárdala.", false),
                    Line("MIXEL", "*Guarda la tarjeta con cuidado.* ¿Por dónde empiezo?", true),
                    Line("ZEST", "Hay un bar no muy lejos de aquí. Thorne & Oak. Es serio, es clásico, y no les gusta que nadie llegue a revolver su orden. Deberías encajar perfecto. La dueña se llama Bastien. No será amable.", false),
                    Line("MIXEL", "Thorne & Oak. Entendido.", true),
                    Line("ZEST", "Trae algo bueno de vuelta, amigo.", false)
                );
            case LevelID.Level2_Bastien:
                return Seq(
                    Line("BASTIEN", "Bien. Tienes técnica. No es perfecta, pero está ahí.", false),
                    Line("BASTIEN", "Old Fashioned. Bourbon, azúcar, amargo de angostura, una piel de naranja. Nada más, nada menos. Así se ha hecho siempre, y así seguirá haciéndose.", false),
                    Line("MIXEL", "*Lo toma. Lo huele antes de beber. Un sorbo largo.* Es… profundo. Serio. Cada elemento en su lugar exacto. Pero no es lo que busco.", true),
                    Line("BASTIEN", "Claro que no. Lo que buscas no se encuentra en la tradición, muchacho. Lo que buscas requiere que primero la conozcas a fondo, y luego la rompas con intención. Guarda eso. Algún día entenderás para qué sirve.", false),
                    Line("MIXEL", "¿Hacia dónde me recomienda seguir?", true),
                    Line("BASTIEN", "Hay otros lugares. Más brillantes, más ruidosos, menos serios. Si puedes tolerar eso, tal vez encuentres algo útil. Hay un bar llamado Cyanara. No es como este. Pero el que lo dirige sabe lo que hace, aunque no le guste admitirlo.", false)
                );
            case LevelID.Level3_EveryLow:
                return Seq(
                    Line("EVERY LOW", "Interesante. Más de lo que esperaba.", false),
                    Line("EVERY LOW", "Blue Lagoon. Vodka, curaçao azul, limón y tónica. Un trago que se ve igual de bien de noche que al fondo del mar.", false),
                    Line("MIXEL", "*Lo levanta. La luz del bar lo atraviesa como si fuera agua real. Bebe.* Es brillante. Casi hipnótico. Un sabor que… llama la atención sin pedir permiso. Pero no es lo que busco.", true),
                    Line("EVERY LOW", "Lo sé. Lo que estás buscando no está en esta zona, muchacho. Guarda eso. Y escucha lo que te digo: si realmente quieres encontrar tu obra perdida, tienes que ir más lejos.", false),
                    Line("EVERY LOW", "La otra zona de la ciudad es completamente distinta. Más libre. Más ruidosa. Menos ordenada. Tal vez allá haya algo que encaje con lo que buscas.", false),
                    Line("MIXEL", "¿Algún nombre en particular?", true),
                    Line("EVERY LOW", "Varios. Pero empieza por The Blooming Veil. Floral, ilusorio, impredecible. Te va a resultar raro al principio. Y ten cuidado. Lo que parece simple en esa zona generalmente no lo es.", false)
                );
            case LevelID.Level4_Nyctagin:
                return Seq(
                    Line("NYCTAGIN", "No muchos logran leer a través de mis copias. Bien jugado.", false),
                    Line("NYCTAGIN", "Purple Haze. Vodka, licor de mora, limón, azúcar. Tan complejo como parece, tan simple como suena.", false),
                    Line("MIXEL", "*Lo toma. Lo mira contra la luz. Bebe despacio.* Es… único. Capas de sabor que aparecen una tras otra. Nunca sabes exactamente qué viene después. Pero tampoco es lo que busco.", true),
                    Line("NYCTAGIN", "Lo suponía. Lo que buscas tiene más historia detrás. Más peso. Guarda eso. Y escúchame: hay un amigo mío en esta zona, alguien que lleva mucho más tiempo que yo en esto. Se llama Rend. Tiene un bar llamado Glass & Grind.", false),
                    Line("NYCTAGIN", "No es… fácil de tratar. Pero sabe cosas que yo no sé. Y además… creo que él puede ayudarte con ese bar al inicio de la zona. El grande, el que no pudiste entrar. Rend tiene conexiones allá.", false),
                    Line("MIXEL", "Glass & Grind. Lo busco.", true),
                    Line("NYCTAGIN", "Dile que vas de mi parte. No prometo que cambie algo, pero al menos te escucha antes de atacar.", false)
                );
            case LevelID.Level5_Rend:
                return Seq(
                    Line("REND", "¡Eso sí que fue bueno! ¡Hacía tiempo que alguien me hacía sudar así! Bien, bien, bien. Te lo mereces, no hay duda.", false),
                    Line("REND", "Dark & Stormy. Ron oscuro, ginger beer, lima. Fuerte, directo, sin pretensiones. Como todo acá.", false),
                    Line("MIXEL", "*Lo toma. Bebe.* Es contundente. Sin disculpas. Un trago que sabe exactamente lo que es. Tampoco es lo que busco.", true),
                    Line("REND", "No me sorprende. Nunca me dijeron que buscabas algo específico. Solo vine aquí a pelear, ¿sabes? Toma eso. Igual te sirve de algo.", false),
                    Line("MIXEL", "El bar de la entrada de la zona. Cinderhall. ¿Puedes ayudarme?", true),
                    Line("REND", "Ah. Ese lugar. Toma. Es un pase. Con eso entras.", false),
                    Line("REND", "Y está la niebla. No te puedo explicar bien qué es, pero cuando la veas, entenderás. Ten cuidado con ella.", false),
                    Line("MIXEL", "¿Algo más que deba saber?", true),
                    Line("REND", "Que si sobrevives, vuelves a pelear conmigo. Esa es la condición. Buena suerte, bartender.", false)
                );
            case LevelID.Level6_Tonicity:
                return Seq(
                    Line("TONIC", "Bien. Muy bien.", false),
                    Line("TONIC", "Gin Tonic. El trago más honesto que existe. No esconde nada.", false),
                    Line("MIXEL", "*Lo toma. Bebe. Y entonces, todo regresa.* Era esto. Era todo esto junto.", true),
                    Line("TONIC", "Lo sabías desde antes del golpe. Solo necesitabas recordar por qué lo sabías.", false),
                    Line("NARRADOR", "No era un cóctel de técnica imposible. No era un elixir de ingredientes raros. Era algo hecho con todo lo que Mixel era: su historia, su memoria, su camino. Un simple, irrepetible y legendario ron con coca.", false)
                );
            default:
                return null;
        }
    }
}
