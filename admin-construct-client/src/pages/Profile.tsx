import { useState, useEffect } from 'react';
// import { useNavigate } from 'react-router-dom';
import { User, Save, ShoppingBag, Calendar, Clock } from 'lucide-react';
import { notifications } from '../utils/notifications';
import api from '../services/api';

interface CustomerProfile {
  id: string;
  name: string;
  document: string;
  email: string;
  phone?: string;
}

interface Purchase {
  id: string;
  date: string;
  totalAmount: number;
  details: {
    product: { name: string };
    quantity: number;
    unitPrice: number;
  }[];
}

interface Rental {
  id: string;
  machinery: { name: string };
  startDateTime: string;
  endDateTime: string;
  totalAmount: number;
  isActive: boolean;
}

export default function Profile() {
  const [profile, setProfile] = useState<CustomerProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [activeTab, setActiveTab] = useState<'profile' | 'purchases' | 'rentals'>('profile');
  const [purchases, setPurchases] = useState<Purchase[]>([]);
  const [rentals, setRentals] = useState<Rental[]>([]);
  
  const [formData, setFormData] = useState({
    name: '',
    document: '',
    phone: ''
  });
  // const navigate = useNavigate();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      
      // Cargar perfil primero (crítico)
      const profileRes = await api.get<CustomerProfile>('/customers/my-profile');
      setProfile(profileRes.data);
      setFormData({
        name: profileRes.data.name || '',
        document: profileRes.data.document || '',
        phone: profileRes.data.phone || ''
      });

      // Cargar historial (no crítico - manejar errores individualmente)
      try {
        const purchasesRes = await api.get<Purchase[]>('/sales');
        setPurchases(purchasesRes.data);
      } catch (error) {
        console.error('Error loading purchases:', error);
        setPurchases([]);
      }

      try {
        const rentalsRes = await api.get<Rental[]>('/machineryrental');
        setRentals(rentalsRes.data);
      } catch (error) {
        console.error('Error loading rentals:', error);
        setRentals([]);
      }

    } catch (error: any) {
      console.error('Error loading profile:', error);
      if (error.response?.status === 404) {
        notifications.error('No se encontró tu perfil. Por favor contacta al administrador.');
      } else if (error.response?.status === 401) {
        notifications.error('Sesión expirada. Por favor inicia sesión nuevamente.');
      } else {
        notifications.error('Error al cargar el perfil');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      await api.put('/customers/my-profile', {
        ...formData,
        email: profile?.email
      });
      notifications.success('Perfil actualizado correctamente');
      // Update local profile state
      if (profile) {
        setProfile({ ...profile, ...formData });
      }
    } catch (error) {
      console.error('Error updating profile:', error);
      notifications.error('Error al actualizar el perfil');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="container mx-auto px-4 max-w-4xl">
        <div className="bg-white rounded-lg shadow-md overflow-hidden">
          {/* Header */}
          <div className="p-6 bg-primary text-white">
            <div className="flex items-center">
              <User className="mr-3" size={32} />
              <div>
                <h1 className="text-2xl font-bold">{profile?.name || 'Mi Perfil'}</h1>
                <p className="text-blue-100">{profile?.email}</p>
              </div>
            </div>
          </div>

          {/* Tabs */}
          <div className="flex border-b border-gray-200">
            <button
              onClick={() => setActiveTab('profile')}
              className={`flex-1 py-4 px-6 text-center font-medium transition ${
                activeTab === 'profile'
                  ? 'text-primary border-b-2 border-primary'
                  : 'text-gray-500 hover:text-primary'
              }`}
            >
              <div className="flex items-center justify-center space-x-2">
                <User size={20} />
                <span>Información Personal</span>
              </div>
            </button>
            <button
              onClick={() => setActiveTab('purchases')}
              className={`flex-1 py-4 px-6 text-center font-medium transition ${
                activeTab === 'purchases'
                  ? 'text-primary border-b-2 border-primary'
                  : 'text-gray-500 hover:text-primary'
              }`}
            >
              <div className="flex items-center justify-center space-x-2">
                <ShoppingBag size={20} />
                <span>Mis Compras</span>
              </div>
            </button>
            <button
              onClick={() => setActiveTab('rentals')}
              className={`flex-1 py-4 px-6 text-center font-medium transition ${
                activeTab === 'rentals'
                  ? 'text-primary border-b-2 border-primary'
                  : 'text-gray-500 hover:text-primary'
              }`}
            >
              <div className="flex items-center justify-center space-x-2">
                <Clock size={20} />
                <span>Mis Alquileres</span>
              </div>
            </button>
          </div>

          {/* Content */}
          <div className="p-6">
            {activeTab === 'profile' && (
              <form onSubmit={handleSubmit} className="space-y-6 max-w-lg mx-auto">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Nombre Completo
                  </label>
                  <input
                    type="text"
                    name="name"
                    value={formData.name}
                    onChange={handleChange}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent"
                    placeholder="Juan Pérez"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Número de Documento
                  </label>
                  <input
                    type="text"
                    name="document"
                    value={formData.document}
                    onChange={handleChange}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent"
                    placeholder="12345678"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Correo Electrónico
                  </label>
                  <input
                    type="email"
                    value={profile?.email || ''}
                    disabled
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg bg-gray-100 cursor-not-allowed"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Teléfono
                  </label>
                  <input
                    type="tel"
                    name="phone"
                    value={formData.phone}
                    onChange={handleChange}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent"
                    placeholder="3001234567"
                  />
                </div>

                <div className="flex items-center space-x-4 pt-4">
                  <button
                    type="submit"
                    disabled={saving}
                    className="flex-1 bg-secondary text-white py-3 rounded-lg font-semibold hover:bg-orange-600 transition disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center space-x-2"
                  >
                    <Save size={20} />
                    <span>{saving ? 'Guardando...' : 'Guardar Cambios'}</span>
                  </button>
                </div>
              </form>
            )}

            {activeTab === 'purchases' && (
              <div className="space-y-4">
                {purchases.length === 0 ? (
                  <div className="text-center py-8 text-gray-500">
                    No has realizado compras aún.
                  </div>
                ) : (
                  purchases.map((purchase) => (
                    <div key={purchase.id} className="border border-gray-200 rounded-lg p-4 hover:shadow-md transition">
                      <div className="flex justify-between items-start mb-4">
                        <div>
                          <p className="text-sm text-gray-500">Fecha: {new Date(purchase.date).toLocaleDateString()}</p>
                          <p className="font-bold text-lg text-primary">Total: ${purchase.totalAmount.toFixed(2)}</p>
                        </div>
                        <span className="bg-green-100 text-green-800 text-xs px-2 py-1 rounded">
                          Completado
                        </span>
                      </div>
                      <div className="border-t border-gray-100 pt-3">
                        <h4 className="text-sm font-medium text-gray-700 mb-2">Detalles:</h4>
                        <ul className="space-y-1">
                          {purchase.details.map((detail, idx) => (
                            <li key={idx} className="text-sm text-gray-600 flex justify-between">
                              <span>{detail.quantity}x {detail.product.name}</span>
                              <span>${(detail.quantity * detail.unitPrice).toFixed(2)}</span>
                            </li>
                          ))}
                        </ul>
                      </div>
                    </div>
                  ))
                )}
              </div>
            )}

            {activeTab === 'rentals' && (
              <div className="space-y-4">
                {rentals.length === 0 ? (
                  <div className="text-center py-8 text-gray-500">
                    No tienes alquileres registrados.
                  </div>
                ) : (
                  rentals.map((rental) => (
                    <div key={rental.id} className="border border-gray-200 rounded-lg p-4 hover:shadow-md transition">
                      <div className="flex justify-between items-start mb-4">
                        <div>
                          <h3 className="font-bold text-lg text-gray-800">{rental.machinery.name}</h3>
                          <div className="flex items-center text-sm text-gray-500 mt-1">
                            <Calendar size={16} className="mr-1" />
                            <span>
                              {new Date(rental.startDateTime).toLocaleDateString()} - {new Date(rental.endDateTime).toLocaleDateString()}
                            </span>
                          </div>
                        </div>
                        <span className={`px-2 py-1 rounded text-xs ${rental.isActive ? 'bg-blue-100 text-blue-800' : 'bg-gray-100 text-gray-800'}`}>
                          {rental.isActive ? 'Activo' : 'Finalizado'}
                        </span>
                      </div>
                      <div className="flex justify-between items-center border-t border-gray-100 pt-3">
                        <span className="text-sm text-gray-600">Total pagado</span>
                        <span className="font-bold text-primary">${rental.totalAmount.toFixed(2)}</span>
                      </div>
                    </div>
                  ))
                )}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
