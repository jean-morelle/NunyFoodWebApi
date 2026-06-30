import { Link } from 'react-router-dom'

export default function HomePage() {
  return (
    <div>
      {/* Hero */}
      <section className="bg-gradient-to-br from-[#16A34A] to-[#15803D] text-white py-24 px-4">
        <div className="max-w-4xl mx-auto text-center">
          <h1 className="text-4xl sm:text-5xl font-bold mb-6 leading-tight">
            Nourrissez vos proches au Togo,<br />
            <span className="text-[#F97316]">où que vous soyez.</span>
          </h1>
          <p className="text-lg sm:text-xl text-green-100 mb-10 max-w-2xl mx-auto">
            Nuny Food vous permet de commander des packs alimentaires pour votre famille au Togo.
            Paiement sécurisé, livraison garantie.
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link
              to="/register"
              className="bg-[#F97316] text-white px-8 py-3 rounded-lg text-lg font-semibold hover:bg-[#EA6C0A] transition-colors"
            >
              Commander maintenant
            </Link>
            <Link
              to="/login"
              className="bg-white text-[#16A34A] px-8 py-3 rounded-lg text-lg font-semibold hover:bg-green-50 transition-colors"
            >
              Se connecter
            </Link>
          </div>
        </div>
      </section>

      {/* How it works */}
      <section className="py-20 px-4 bg-white">
        <div className="max-w-5xl mx-auto">
          <h2 className="text-3xl font-bold text-center text-gray-900 mb-12">Comment ça marche ?</h2>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-8">
            {[
              { step: '1', title: 'Choisissez un pack', desc: 'Sélectionnez parmi nos packs alimentaires adaptés aux besoins de votre famille.' },
              { step: '2', title: 'Payez en ligne', desc: "Paiement sécurisé via PayPal, TMoney ou Flooz depuis n'importe où dans le monde." },
              { step: '3', title: 'Livraison garantie', desc: 'Nos livreurs livrent directement chez votre bénéficiaire avec confirmation photo.' },
            ].map(({ step, title, desc }) => (
              <div key={step} className="text-center p-6">
                <div className="w-14 h-14 bg-[#16A34A] text-white rounded-full flex items-center justify-center text-2xl font-bold mx-auto mb-4">
                  {step}
                </div>
                <h3 className="text-lg font-semibold text-gray-900 mb-2">{title}</h3>
                <p className="text-gray-500 text-sm leading-relaxed">{desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Pack examples */}
      <section className="py-20 px-4 bg-gray-50">
        <div className="max-w-5xl mx-auto">
          <h2 className="text-3xl font-bold text-center text-gray-900 mb-12">Nos packs populaires</h2>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-6">
            {[
              { name: 'Pack Étudiant', items: ['Riz 5 kg', 'Huile 1 L', 'Sucre 2 kg'], price: '25 000 FCFA' },
              { name: 'Pack Famille', items: ['Riz 25 kg', 'Huile 5 L', 'Sucre 5 kg', 'Pâtes'], price: '55 000 FCFA' },
              { name: 'Pack Premium', items: ['Produits variés', 'Sélection qualité', 'Quantités supérieures'], price: '95 000 FCFA' },
            ].map(({ name, items, price }) => (
              <div key={name} className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
                <div className="h-36 bg-gradient-to-br from-green-50 to-green-100 flex items-center justify-center">
                  <span className="text-5xl">🛒</span>
                </div>
                <div className="p-5">
                  <h3 className="text-base font-semibold text-gray-900 mb-2">{name}</h3>
                  <ul className="space-y-1 mb-4">
                    {items.map((item) => (
                      <li key={item} className="text-sm text-gray-500 flex items-center gap-1.5">
                        <span className="text-[#16A34A]">✓</span> {item}
                      </li>
                    ))}
                  </ul>
                  <p className="text-[#16A34A] font-bold text-lg">{price}</p>
                </div>
              </div>
            ))}
          </div>
          <div className="text-center mt-10">
            <Link
              to="/register"
              className="inline-block bg-[#16A34A] text-white px-8 py-3 rounded-lg font-semibold hover:bg-[#15803D] transition-colors"
            >
              Voir tous les packs
            </Link>
          </div>
        </div>
      </section>
    </div>
  )
}
